var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
