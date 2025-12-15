using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BidService.DTO;

namespace BidService.DTO
{
    public class CreateBidRequest
    {
        public Guid AuctionId { get; set; }
        public decimal Amount { get; set; }
    }
}