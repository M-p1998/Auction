using System.Text;
using AuctionService.Auth;
using AuctionService.Data;
using AuctionService.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MassTransit;


var builder = WebApplication.CreateBuilder(args);

// Add Controllers + FluentValidation
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddHttpClient<SearchSyncService>();

var rabbitSection = builder.Configuration.GetSection("RabbitMq");
var rabbitHost = rabbitSection["Host"];
var rabbitUser = rabbitSection["Username"];
var rabbitPass = rabbitSection["Password"];
builder.Services.AddMassTransit(x =>
{

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });

        // Optional: global retry on consumers hosted here (if any)
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<InitialMongoSync>();

builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// 🔐 JWT Authentication Setup
// var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]);
// var jwtKey = builder.Configuration["JwtSettings:Key"] ?? "DEV_SECRET_KEY_123456789";
var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]);


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
    using (var scope = app.Services.CreateScope())
    {
        var sync = scope.ServiceProvider.GetRequiredService<InitialMongoSync>();
        await sync.SyncAllAsync();
    }
}
catch (Exception e)
{
    Console.WriteLine(e);
}


app.Run();





// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddControllers();

// // JWT Authentication
// var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]);

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = false,
//             ValidateAudience = false,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey = new SymmetricSecurityKey(key),
//             ClockSkew = TimeSpan.Zero
//         };
//     });

// // Authorization
// builder.Services.AddAuthorization();

// var app = builder.Build();

// app.UseAuthentication();
// app.UseAuthorization();

// app.MapControllers();

// if (app.Environment.IsDevelopment())
// {
//     DbInitializer.InitDb(app);
// }

// app.Run();
