using BidService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddDbContext<BidDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AuctionCreatedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"]);
            h.Password(builder.Configuration["RabbitMq:Password"]);
        });
        cfg.ConfigureEndpoints(context);
    });
});

var jwtKey = builder.Configuration["JwtSettings:Key"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!)
            )
        };
    });
builder.Services.AddHttpClient("auction", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AuctionServiceUrl"]!);
});


builder.Services.AddAuthorization();


var app = builder.Build();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

// auto migrate 
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<BidDbContext>();
db.Database.Migrate();

app.Run();
