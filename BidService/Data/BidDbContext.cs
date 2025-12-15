using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BidService.Entities;

namespace BidService.Data
{
    public class BidDbContext : DbContext
    {
        public BidDbContext(DbContextOptions<BidDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bid> Bids { get; set; }
    }
}