using Catalog.Infrastructure;
using Catalog.Infrastructure.Data;
using Catalog.Application;
using Foundation.Exceptions;
using Carter;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.Services
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices();

builder.Services.AddControllers();
builder.Services.AddCarter();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("https://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8081, listenOptions =>
    {
        listenOptions.UseHttps(httpsOptions =>
        {
            httpsOptions.ServerCertificateSelector = (context, name) =>
            {
                if (name == "catalog-api")
                {
                    return X509Certificate2.CreateFromPemFile(
                        "/https/catalog-api.crt",
                        "/https/catalog-api.key");
                }
                else
                {
                    return new X509Certificate2("/https/localhost.pfx", "111");
                }
            };
        });
    });
});

var app = builder.Build();
app.MapCarter();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors("AllowSpecificOrigin");

app.UseExceptionHandler(options => { });
await app.InitialiseDatabaseAsync();

app.Run();
