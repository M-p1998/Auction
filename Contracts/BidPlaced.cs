using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts
{
    public class BidPlaced
    {
        public Guid AuctionId { get; set; }
        public string Bidder { get; set; } // email
        public decimal Amount { get; set; }
    }
}