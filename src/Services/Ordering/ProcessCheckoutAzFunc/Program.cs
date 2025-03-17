using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProcessCheckoutAzFunc;
using Refit;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddHttpClients();

builder.ConfigureFunctionsWebApplication();

builder.Build().Run();
