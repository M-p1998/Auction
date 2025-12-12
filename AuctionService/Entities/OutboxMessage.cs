using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
        public string Type { get; set; } = default!;      
        public string Content { get; set; } = default!;  
        public DateTime? ProcessedOn { get; set; }
    }
}