using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using ProcessCheckoutAzFunc;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddHttpClients();
builder.ConfigureFunctionsWebApplication();

builder.Build().Run();
