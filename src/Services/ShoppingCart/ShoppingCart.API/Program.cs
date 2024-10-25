using Carter;
using FluentValidation;
using Foundation.Exceptions;
using Marten;
using Messaging.MassTransit;
using Newtonsoft.Json.Serialization;
using Refit;
using ShoppingCart.API;
using ShoppingCart.API.Repository;
using ShoppingCart.API.Services;
using ShoppingCart.API.Validation;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    });

builder.Services.AddCarter();

builder.Services.AddSingleton<IDocumentStore>(provider =>
{
    return DocumentStore.For(opts =>
    {
        opts.Connection(builder.Configuration.GetConnectionString("Database")!);
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

builder.Services.AddRefitClient<ICatalogService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler(() => new LoggingHandler());

builder.Services.AddMediatR(config =>
    {
        config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
       // config.AddBehavior(typeof(ValidationBehavior<,>));
    });

builder.Services.AddValidatorsFromAssemblyContaining<CheckoutCartRequestValidator>();
//builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddMessageBroker(builder.Configuration, Assembly.GetExecutingAssembly());

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder => builder
            .WithOrigins("https://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();
var app = builder.Build();
app.UseSession();
app.MapCarter();
app.MapControllers();
app.UseStaticFiles();
//app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowSpecificOrigins");
app.UseExceptionHandler(options => { });

app.Run();
