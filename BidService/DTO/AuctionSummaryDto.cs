using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BidService.DTO
{
    public class AuctionSummaryDto
    {
        public int ReservePrice { get; set; }
        public decimal CurrentHighBid { get; set; }
        public string Status { get; set; } = "";
    }
}