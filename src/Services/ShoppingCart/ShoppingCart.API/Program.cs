using Carter;
using Foundation.Exceptions;
using Marten;
using Refit;
using ShoppingCart.API;
using ShoppingCart.API.Repository;
using ShoppingCart.API.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddCarter();

builder.Services.AddSingleton<IDocumentStore>(provider =>
{
    var store = DocumentStore.For(opts =>
    {
        opts.Connection(builder.Configuration.GetConnectionString("Database")!);
        opts.Schema.Include<CartConfiguration>();
    });

    return store;
});
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddOutputCache();

builder.Services.AddRefitClient<ICatalogService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler(() => new LoggingHandler()); 

builder.Services.AddMediatR(config =>
    {
        config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    });

builder.Services.AddStackExchangeRedisOutputCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:RedisConnectionString");
    options.InstanceName = "shoppingcart-api_";
});

builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("Cache10Minutes", policyBuilder =>
    {
        policyBuilder.Expire(TimeSpan.FromMinutes(10));
    });
});

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
app.MapCarter();
app.UseOutputCache();
app.MapControllers();
app.UseStaticFiles();
//app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowSpecificOrigins");
app.UseExceptionHandler(options => { });

app.Run();
