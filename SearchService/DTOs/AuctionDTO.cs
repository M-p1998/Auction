using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchService.DTOs
{
    public class AuctionDTO
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public int Year { get; set; }
        public int Mileage { get; set; }
        public string ImageUrl { get; set; }
        public int ReservePrice { get; set; }
        public DateTime AuctionEnd { get; set; }
    }
}