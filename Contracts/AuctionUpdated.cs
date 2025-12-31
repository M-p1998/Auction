using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts
{
    public class AuctionUpdated
    {
        public Guid Id { get; set; }
        public int ReservePrice { get; set; }
        public decimal CurrentHighBid { get; set; }
        public DateTime AuctionEnd { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public int Mileage { get; set; }
        public string ImageUrl { get; set; }
    }
}