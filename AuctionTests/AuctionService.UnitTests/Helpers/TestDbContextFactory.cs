using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Data;
using Microsoft.EntityFrameworkCore;


namespace AuctionService.UnitTests.Helpers
{
    public static class TestDbContextFactory
    {
        public static AuctionDbContext Create()
        {
            var options = new DbContextOptionsBuilder<AuctionDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AuctionDbContext(options);
        }
    }
}