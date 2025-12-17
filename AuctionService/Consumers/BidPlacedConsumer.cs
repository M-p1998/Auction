using Contracts;
using MassTransit;
using AuctionService.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Contracts.Enums;

namespace AuctionService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly AuctionDbContext _context;

    public BidPlacedConsumer(AuctionDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        var auction = await _context.Auctions
            .FirstOrDefaultAsync(x => x.Id == context.Message.AuctionId);

        if (auction == null) return;
        // Ignore bids after auction end
        if (auction.Status != AuctionStatus.Live) return;

        if (context.Message.Amount > auction.CurrentHighBid)
        {
            auction.CurrentHighBid = context.Message.Amount;
            auction.WinningBidder = context.Message.Bidder;
            auction.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

  
}
