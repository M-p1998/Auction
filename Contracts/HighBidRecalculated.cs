using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts
{
    public class HighBidRecalculated
    {
        public Guid AuctionId { get; set; }
        public decimal CurrentHighBid { get; set; }
    }
}