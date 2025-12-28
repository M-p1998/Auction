using System.Collections.Generic;
using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;


namespace AuctionService.Data;

public class AuctionDbContext : DbContext
{
    public AuctionDbContext(DbContextOptions options) : base(options)
    {
        
    }

    public DbSet<Auction> Auctions { get; set; }    
    public DbSet<User> Users { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<Bid> Bids { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Auction>()
            .Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
    }

}