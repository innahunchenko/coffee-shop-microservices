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
using Foundation.Abstractions.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
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


var app = builder.Build();
app.UseRouting();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowSpecificOrigins");
app.UseExceptionHandler(options => { });
app.InitialiseDatabaseAsync<AppDbContext>();
app.MapFallbackToFile("index.html");
app.Run();
