using Auth.API.Data;
using Auth.API.Services;
using Auth.API.Repositories;
using Foundation.Abstractions;
using Microsoft.EntityFrameworkCore;
using Foundation.Exceptions;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Auth.API.Domain.Models;
using FluentValidation;
using MediatR;
using Auth.API.Validation;
using Foundation.Abstractions.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Auth.API.OptionsSetup;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<IDbContext, AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddIdentity<CoffeeShopUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IDbContext, AppDbContext>();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder => builder
            .WithOrigins("https://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Must be specified after AddIdentity
builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8081, listenOptions =>
    {
        listenOptions.UseHttps(httpsOptions =>
        {
            httpsOptions.ServerCertificateSelector = (context, name) =>
            {
                if (name == "auth-api")
                {
                    return X509Certificate2.CreateFromPemFile(
                        "/https/auth-api.crt",
                        "/https/auth-api.key");
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

//app.Use(async (context, next) =>
//{
//    if (context.Request.Headers.ContainsKey("Authorization"))
//    {
//        var authHeader = context.Request.Headers["Authorization"].ToString();
//        Console.WriteLine($"Authorization header found: {authHeader}");
//    }
//    else
//    {
//        Console.WriteLine("No Authorization header found.");
//    }

//    await next();
//});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowSpecificOrigins");
app.UseExceptionHandler(options => { });
app.InitialiseDatabaseAsync<AppDbContext>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();
