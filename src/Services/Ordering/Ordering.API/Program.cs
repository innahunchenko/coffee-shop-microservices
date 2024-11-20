using Foundation.Exceptions;
using Messaging.MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Ordering.API.Data;
using Ordering.API.Data.Interceptors;
using Ordering.API.Repositories;
using Ordering.API.Services;
using RedisCachingService;
using StackExchange.Redis;
using System.Data;
using System.Reflection;
using Foundation.Abstractions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddControllers();
var redisConnectionString = builder.Configuration.GetValue<string>("CacheSettings:RedisConnectionString");
var expiryTime = TimeSpan.FromMinutes(builder.Configuration.GetValue<double>("CacheSettings:DefaultCacheDurationMinutes"));
var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString!);
builder.Services.AddSingleton(connectionMultiplexer);
builder.Services.AddScoped<IRedisCacheRepository, RedisCacheRepository>(provider =>
{
    var multiplexer = provider.GetRequiredService<ConnectionMultiplexer>();
    return new RedisCacheRepository(multiplexer, expiryTime);
});

var connectionString = builder.Configuration.GetConnectionString("OrdersConnection");
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
//builder.Services.AddScoped<IOrderCacheService, OrderCacheService>();
//builder.Services.Decorate<IOrderService, OrderServiceCacheDecorator>();

builder.Services.AddScoped<ISaveChangesInterceptor, SaveEntityInterceptor>();
builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IDbContext, AppDbContext>();
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));


builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
   // config.AddBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddMessageBroker(builder.Configuration, Assembly.GetExecutingAssembly());

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder => builder
            .WithOrigins("https://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();
app.UseCors("AllowSpecificOrigins");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.UseExceptionHandler(options => { });
app.InitialiseDatabaseAsync<AppDbContext>();
app.Run();
