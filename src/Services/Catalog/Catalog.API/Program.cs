using Catalog.Infrastructure;
using Catalog.Infrastructure.Data;
using Catalog.Application;
using Foundation.Exceptions;
using Carter;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices();

builder.Services.AddControllers();
builder.Services.AddCarter();
builder.Services.AddOutputCache();

builder.Services.AddStackExchangeRedisOutputCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:RedisConnectionString");
    options.InstanceName = "catalog-api_";
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();
app.MapCarter();
app.UseResponseCaching();
app.UseStaticFiles();
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors("AllowSpecificOrigin");

app.UseExceptionHandler(options => { });
app.UseOutputCache();
await app.InitialiseDatabaseAsync();

app.Run();
