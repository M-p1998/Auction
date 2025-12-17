using System;
using Contracts.Enums;

namespace AuctionService.Entities;

public class Auction
{
    public Guid Id { get; set; }
    public int ReservePrice { get; set; }
    public string Seller { get; set; }
    public string Winner { get; set; }
    public string WinningBidder { get; set; }
    public decimal SoldAmount { get; set; }
    public decimal CurrentHighBid { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime AuctionEnd { get; set; }

    public AuctionStatus Status { get; set; } = AuctionStatus.Live;

    public Item Item { get; set; }
}
