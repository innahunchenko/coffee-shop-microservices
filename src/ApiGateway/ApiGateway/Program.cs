using Foundation.Abstractions.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

//builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

//builder.Services.AddAuthentication(x =>
//{
//    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions!.Secret)),
//        ValidateIssuer = true,
//        ValidIssuer = jwtOptions.Issuer,
//        ValidAudience = jwtOptions.Audience,
//        ValidateAudience = true
//    };
//});

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("JwtPolicy", policy =>
//        policy.RequireAuthenticatedUser());
//});

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
//app.UseAuthentication();
//app.UseAuthorization();
app.MapReverseProxy();
//app.UseHttpsRedirection();

app.Run();
