using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Entities;

namespace AuctionService.Services
{
    public class SearchSyncService
    {
        private readonly HttpClient _http;

        public SearchSyncService(HttpClient http)
        {
            _http = http;
        }

        public async Task SyncWithSearchService(Auction auction)
        {
            var dto = new
            {
                auction.Item.Make,
                auction.Item.Model,
                auction.Item.Color,
                auction.Item.Year,
                auction.Item.Mileage,
                auction.Item.ImageUrl,
                auction.AuctionEnd,
                auction.ReservePrice
            };

            await _http.PostAsJsonAsync("http://localhost:7002/api/search/upsert", dto);
        }
    }
}