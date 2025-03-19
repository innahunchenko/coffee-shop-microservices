using Azure.Messaging.ServiceBus;
using Carter;
using FluentValidation;
using Foundation.Abstractions.Services;
using Foundation.Exceptions;
using Marten;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
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

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    });

builder.Services.AddCarter();

Console.WriteLine(builder.Configuration.GetConnectionString("Database")!);

builder.Services.AddHostedService<MartenWarmupService>();

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

//builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
//{
//    var configuration = builder.Configuration["CacheSettings:RedisConnectionString"];
//    return ConnectionMultiplexer.Connect(configuration!);
//});

//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = builder.Configuration["CacheSettings:RedisConnectionString"];
//    options.InstanceName = "SessionCache_";
//});

var connectionString = builder.Configuration.GetConnectionString("SqlDatabase");

CreateSessionTable(connectionString!);

builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = connectionString;
    options.SchemaName = "dbo";
    options.TableName = "SessionData";
});

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "CoffeeShop.Session";
    options.IdleTimeout = TimeSpan.FromHours(48);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
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

var config = builder.Configuration;
builder.Services.AddSingleton(sp =>
{
    return new ServiceBusClient(config["ServiceBus:ConnectionString"]);
});

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

// Регистрация прогрева как HostedService
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

void CreateSessionTable(string connectionString)
{
    var script = @"
    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'SessionData' AND xtype = 'U')
    BEGIN
        CREATE TABLE [dbo].[SessionData](
            [Id] NVARCHAR(449) NOT NULL PRIMARY KEY, 
            [Value] VARBINARY(MAX) NOT NULL, 
            [ExpiresAtTime] DATETIMEOFFSET NOT NULL, 
            [SlidingExpirationInSeconds] BIGINT NULL, 
            [AbsoluteExpiration] DATETIMEOFFSET NULL
        );
    END
";

    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();
        using (var command = new SqlCommand(script, connection))
        {
            command.ExecuteNonQuery();
        }
    }
}