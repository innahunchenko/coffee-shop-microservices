using Foundation.Exceptions;
using Messaging.MassTransit;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

builder.Services.AddMessageBroker(builder.Configuration);

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

app.MapControllers();
app.UseStaticFiles();
app.UseAuthorization();
app.UseCors("AllowSpecificOrigins");
app.UseExceptionHandler(options => { });

app.Run();
