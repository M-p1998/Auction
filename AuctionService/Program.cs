using System.Text;
using AuctionService.Auth;
using AuctionService.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers + FluentValidation
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddFluentValidationAutoValidation();

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
    DbInitializer.InitDb(app);
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
