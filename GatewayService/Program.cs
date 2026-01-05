using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Production.json", optional: true)
    .AddJsonFile("/app/config/appsettings.Production.json", optional: true)
    .AddEnvironmentVariables();


// JWT Authentication
// var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]);
var jwtKey = builder.Configuration["JwtSettings:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("JwtSettings:Key is missing");
}

var key = Encoding.UTF8.GetBytes(jwtKey);


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

// YARP Reverse Proxy
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();  

var app = builder.Build();

// CORS before proxy
app.UseCors("AllowFrontend");
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

// app.UseAuthentication();
// app.UseAuthorization();

// Route everything through Gateway
app.MapReverseProxy();

app.Run();
