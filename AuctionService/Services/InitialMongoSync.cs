using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Data;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Services
{
    public class InitialMongoSync
    {
        private readonly AuctionDbContext _context;
        private readonly SearchSyncService _syncService;

        public InitialMongoSync(AuctionDbContext context, SearchSyncService syncService)
        {
            _context = context;
            _syncService = syncService;
        }

        public async Task SyncAllAsync()
        {
            var allAuctions = await _context.Auctions
                .Include(a => a.Item)
                .ToListAsync();

            foreach (var auction in allAuctions)
            {
                await _syncService.SyncWithSearchService(auction);
            }
        }
    }
}