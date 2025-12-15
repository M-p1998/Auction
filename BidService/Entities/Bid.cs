using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BidService.Entities
{
    public class Bid
    {
        public Guid Id { get; set; }
        public Guid AuctionId { get; set; }
        public string Bidder { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}