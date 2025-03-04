using Catalog.Infrastructure;
using Catalog.Infrastructure.Data;
using Catalog.Application;
using Carter;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
var connectionString = builder.Configuration.GetConnectionString("ProductsConnection");
Console.WriteLine(connectionString);

builder.Services
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices();

builder.Services.AddControllers();
builder.Services.AddCarter();

builder.Configuration.AddEnvironmentVariables();


//builder.Services.AddExceptionHandler<CustomExceptionHandler>();

//builder.WebHost.ConfigureKestrel(options =>
//{
//    var certificatePassword = builder.Configuration["Kestrel:Certificates:Default:Password"];
//    var certificatePath = builder.Configuration["Kestrel:Certificates:Default:Path"]!;
//    var defaultCertificate = new X509Certificate2(certificatePath, certificatePassword);
//    options.ListenAnyIP(8081, listenOptions =>
//    {
//        listenOptions.UseHttps(httpsOptions =>
//        {
//            httpsOptions.ServerCertificateSelector = (context, name) =>
//            {
//                if (name == "catalog-api")
//                {
//                    return X509Certificate2.CreateFromPemFile(
//                        "/https/catalog-api.crt",
//                        "/https/catalog-api.key");
//                }
//                else
//                {
//                    return defaultCertificate;
//                }
//            };
//        });
//    });
//});

var app = builder.Build();
//app.MapGet("/", () => Results.Redirect("/categories"));
app.UseCors("AllowSpecificAndDynamicOrigins");
app.MapCarter();
app.UseStaticFiles();
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseExceptionHandler(options => { });

await app.InitialiseDatabaseAsync();

app.Run();

Console.WriteLine("FROM CATALOG API!!");
