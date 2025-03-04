using ApiGateway;
using Foundation.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using Yarp.ReverseProxy.Configuration;
var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddReverseProxy()
//    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

//var routes = GetRoutes().ToList();
//var clusters = GetClusters().ToList();

//builder.Services.AddReverseProxy()
//    .LoadFromMemory(routes, clusters);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables(); 

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

//builder.WebHost.ConfigureKestrel(options =>
//{
//    var certificatePassword = builder.Configuration["Kestrel:Certificates:Default:Password"];
//    var certificatePath = builder.Configuration["Kestrel:Certificates:Default:Path"]!;
//    var defaultCertificate = new X509Certificate2(certificatePath, certificatePassword);
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
//});

var app = builder.Build();
app.UseCors("AllowSpecificAndDynamicOrigins");
app.UseMiddleware<TokenMiddleware>();
app.UseRouting();

app.MapReverseProxy();
//app.UseHttpsRedirection();

Console.WriteLine(builder.Configuration.GetValue<string>("ReverseProxy__Clusters__catalog-cluster__Destinations__destination1__Address"));
//Console.WriteLine(builder.Configuration.GetValue<string>("ReverseProxy__Clusters__shoppingCart-cluster__Destinations__destination1__Address"));


var proxyConfigProvider = app.Services.GetRequiredService<IProxyConfigProvider>();
var config = proxyConfigProvider.GetConfig();
foreach (var cluster in config.Clusters)
{
    Console.WriteLine($"Cluster: {cluster.ClusterId}");
    foreach (var destination in cluster.Destinations)
    {
        Console.WriteLine($"  Destination: {destination.Key}, Address: {destination.Value.Address}");
    }
}

app.Run();

Console.WriteLine("FROM API GATEWAY!!");

/*
static IEnumerable<RouteConfig> GetRoutes()
{
    return
    [
        new RouteConfig
        {
            RouteId = "catalog-route",
            ClusterId = "catalog-cluster",
            Match = new RouteMatch
            {
                Path = "/catalog/{**catch-all}"
            }
        },
        new RouteConfig
        {
            RouteId = "shoppingCart-route",
            ClusterId = "shoppingCart-cluster",
            Match = new RouteMatch
            {
                Path = "/cart/{**catch-all}"
            }
        },
        new RouteConfig
        {
            RouteId = "ordering-route",
            ClusterId = "ordering-cluster",
            Match = new RouteMatch
            {
                Path = "/ordering/{**catch-all}"
            }
        },
        new RouteConfig
        {
            RouteId = "auth-route",
            ClusterId = "auth-cluster",
            Match = new RouteMatch
            {
                Path = "/auth/{**catch-all}"
            }
        }
    ];
}

static IReadOnlyList<ClusterConfig> GetClusters()
{
    return new List<ClusterConfig>
    {
        new ClusterConfig
        {
            ClusterId = "catalog-cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "destination1", new DestinationConfig { Address = GetEnvironmentVariable("CATALOG_API_URL") } }
            }
        },
        new ClusterConfig
        {
            ClusterId = "shoppingCart-cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "destination1", new DestinationConfig { Address = GetEnvironmentVariable("SHOPPING_CART_API_URL") } }
            }
        },
        new ClusterConfig
        {
            ClusterId = "ordering-cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "destination1", new DestinationConfig { Address = GetEnvironmentVariable("ORDERING_API_URL") } }
            }
        },
        new ClusterConfig
        {
            ClusterId = "auth-cluster",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "destination1", new DestinationConfig { Address = GetEnvironmentVariable("AUTH_API_URL") } }
            }
        }
    };
}

static string GetEnvironmentVariable(string variableName)
{
    return Environment.GetEnvironmentVariable(variableName) 
        ?? throw new InvalidOperationException($"Environment variable {variableName} is not set.");
}
*/
