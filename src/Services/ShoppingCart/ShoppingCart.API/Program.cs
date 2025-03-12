using Carter;
using FluentValidation;
using Foundation.Abstractions.Services;
using Foundation.Exceptions;
using Marten;
using Messaging.MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json.Serialization;
using Refit;
using Security.OptionsSetup;
using Security.Services;
using ShoppingCart.API;
using ShoppingCart.API.Repository;
using ShoppingCart.API.Services;
using ShoppingCart.API.Validation;
using StackExchange.Redis;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    });

builder.Services.AddCarter();

builder.Services.AddSingleton<IDocumentStore>(provider =>
{
    var connectionString = $"Host={Environment.GetEnvironmentVariable("PGHOST")};" +
                           $"Username={Environment.GetEnvironmentVariable("PGUSER")};" +
                           $"Password={Environment.GetEnvironmentVariable("PGPASSWORD")};" +
                           $"Port={Environment.GetEnvironmentVariable("PGPORT")};" +
                           $"Database={Environment.GetEnvironmentVariable("PGDATABASE")}";

    return DocumentStore.For(opts =>
    {
        opts.Connection(connectionString);
        opts.Schema.Include<CartConfiguration>();
    });
});

builder.Services.AddScoped(provider =>
{
    var documentStore = provider.GetRequiredService<IDocumentStore>();
    return documentStore.LightweightSession();
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration["CacheSettings:RedisConnectionString"];
    return ConnectionMultiplexer.Connect(configuration!);
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["CacheSettings:RedisConnectionString"];
    options.InstanceName = "SessionCache_";
});

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "CoffeeShop.Session";
    options.IdleTimeout = TimeSpan.FromHours(48);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<ISecureTokenService, SecureTokenService>();

builder.Services.AddRefitClient<ICatalogService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler(() => new LoggingHandler());

builder.Services.AddMediatR(config =>
    {
        config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    });

builder.Services.AddValidatorsFromAssemblyContaining<CheckoutCartRequestValidator>();

builder.Services.AddMessageBroker(builder.Configuration, Assembly.GetExecutingAssembly());

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer();

var app = builder.Build();
app.UseCors("AllowSpecificAndDynamicOrigins");
app.UseSession();
app.MapCarter();
app.MapControllers();
app.UseStaticFiles();
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler(options => { });

app.Run();
