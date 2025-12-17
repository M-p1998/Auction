using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Enums;

namespace BidService.Entities
{
    public class AuctionSnapshot
    {
         public Guid Id { get; set; }
        public int ReservePrice { get; set; }
        public DateTime AuctionEnd { get; set; }
        public AuctionStatus Status { get; set; } = AuctionStatus.Live;
    }
}