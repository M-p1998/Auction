using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// JWT Authentication
var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]);

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
                            
var app = builder.Build();
builder.Services.AddHealthChecks();

// CORS before proxy
app.UseCors("AllowFrontend");
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

// app.UseAuthentication();
// app.UseAuthorization();

// Route everything through Gateway
app.MapReverseProxy();

app.Run();
