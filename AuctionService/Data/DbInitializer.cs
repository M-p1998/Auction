using AuctionService.Entities;
using AuctionService.Helpers;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;

public class DbInitializer
{
    public static void InitDb(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        
        SeedData(scope.ServiceProvider.GetRequiredService<AuctionDbContext>());
    }

    private static void SeedData(AuctionDbContext context)
    {
        context.Database.Migrate();

        if (!context.Admins.Any())
        {
            var admin = new Admin
            {
                Email = "admin@auction.com",
                PasswordHash = PasswordHasher.HashPassword("Admin#123")
            };

            context.Admins.Add(admin);
            context.SaveChanges();
        }
    }
}