using ApiGateway;
using Foundation.Abstractions.Services;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddHttpContextAccessor();
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddSingleton<ICookieService, CookieService>();
//var allowedOrigins = builder.Configuration["ALLOWED_ORIGINS"]?.Split(',') ?? [];
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificAndDynamicOrigins", builder =>
//    {
//        builder.WithOrigins(allowedOrigins)
//        .AllowAnyMethod()
//        .AllowAnyHeader()
//        .AllowCredentials();
//    });
//});

builder.WebHost.ConfigureKestrel(options =>
{
    var certificatePassword = builder.Configuration["Kestrel:Certificates:Default:Password"];
    var certificatePath = builder.Configuration["Kestrel:Certificates:Default:Path"]!;
    var defaultCertificate = new X509Certificate2(certificatePath, certificatePassword);
    //options.ListenAnyIP(8081, listenOptions =>
    //{
    //    listenOptions.UseHttps(httpsOptions =>
    //    {
    //        httpsOptions.ServerCertificateSelector = (context, name) =>
    //        {
    //            if (name == "api-gateway")
    //            {
    //                return X509Certificate2.CreateFromPemFile(
    //                    "/https/api-gateway.crt",
    //                    "/https/api-gateway.key");
    //            }
    //            else
    //            {
    //                return defaultCertificate;
    //            }
    //        };
    //    });
    //});
});

var app = builder.Build();
app.UseCors("AllowSpecificAndDynamicOrigins");
app.UseMiddleware<TokenMiddleware>();
app.UseRouting();

app.MapReverseProxy();
//app.UseHttpsRedirection();

app.Run();
