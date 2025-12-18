using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts
{
    public class AuctionEnded
    {
        public Guid Id { get; set; }
        public string Winner { get; set; } = string.Empty;
        public decimal SoldAmount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}