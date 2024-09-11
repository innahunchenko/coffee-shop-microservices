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
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors("AllowSpecificOrigin");

app.UseExceptionHandler(options => { });

await app.InitialiseDatabaseAsync();

app.Run();
