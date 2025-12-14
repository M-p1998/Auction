using System.Text;
using AuctionService.Auth;
using AuctionService.Data;
// using AuctionService.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MassTransit;
using Contracts;
using AuctionService.Services;


var builder = WebApplication.CreateBuilder(args);

// Add Controllers + FluentValidation
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddFluentValidationAutoValidation();

// builder.Services.AddHttpClient<SearchSyncService>();

// builder.Services.AddScoped<InitialMongoSync>();

builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    // options.InstanceName = "auction_";
});
builder.Services.AddHostedService<OutboxPublisher>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// 🔐 JWT Authentication Setup
// var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]);
// var jwtKey = builder.Configuration["JwtSettings:Key"] ?? "DEV_SECRET_KEY_123456789";
var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]);

// builder.Services.AddMassTransit(x =>
// {
//     x.UsingRabbitMq((context, cfg) =>
//     {
//         cfg.Host(builder.Configuration["RabbitMq:Host"], "/", h =>
//         {
//             h.Username(builder.Configuration["RabbitMq:Username"]);
//             h.Password(builder.Configuration["RabbitMq:Password"]);
//         });
//     });
// });

builder.Services.AddMassTransit(x =>
{
    // register all consumers from this service (even if none exists)
    // x.AddConsumersFromNamespaceContaining<Program>();
    // x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();


    // x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"]);
            h.Password(builder.Configuration["RabbitMq:Password"]);
        });

        cfg.ConfigureEndpoints(context);
        // cfg.UseEntityFrameworkOutbox<AuctionDbContext>(context);
    });
});



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();  
app.UseAuthorization();

app.MapControllers();

try
{
    // Initialize PostgreSQL DB
    DbInitializer.InitDb(app);

// Initial MongoDB Sync
    // using (var scope = app.Services.CreateScope())
    // {
    //     var sync = scope.ServiceProvider.GetRequiredService<InitialMongoSync>();
    //     await sync.SyncAllAsync();
    // }
}
catch (Exception e)
{
    Console.WriteLine(e);
}


app.Run();




