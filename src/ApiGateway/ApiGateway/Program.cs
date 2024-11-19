using ApiGateway;
using Foundation.Abstractions.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddHttpContextAccessor();
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddSingleton<ICookieService, CookieService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("https://localhost:4200")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});

var app = builder.Build();
app.UseCors("AllowSpecificOrigins");
app.UseMiddleware<TokenMiddleware>();
app.UseRouting();

app.MapReverseProxy();
app.UseHttpsRedirection();

app.Run();
