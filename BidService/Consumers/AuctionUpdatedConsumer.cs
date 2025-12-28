using Contracts;
using MassTransit;
using BidService.Data;
using Microsoft.EntityFrameworkCore;


public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
{
    private readonly BidDbContext _context;

    public AuctionUpdatedConsumer(BidDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        var auctionId = context.Message.Id;
        var reservePrice = context.Message.ReservePrice;

        var validHighBid = await _context.Bids
            .Where(b => b.AuctionId == auctionId &&
                        b.Amount >= reservePrice)
            .OrderByDescending(b => b.Amount)
            .Select(b => (decimal?)b.Amount)
            .FirstOrDefaultAsync();

        // var resultEvent = new HighBidRecalculated
        // {
        //     AuctionId = auctionId,
        //     CurrentHighBid = validHighBid ?? 0
        // };

        // await context.Publish(resultEvent);
        await context.Publish(new HighBidRecalculated
        {
            AuctionId = auctionId,
            CurrentHighBid = validHighBid ?? 0
        });
    }
}
