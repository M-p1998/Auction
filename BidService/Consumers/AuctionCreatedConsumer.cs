using BidService.Data;
using BidService.Entities;
using Contracts;
using MassTransit;
using Contracts.Enums;


public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    private readonly BidDbContext _context;

    public AuctionCreatedConsumer(BidDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        var message = context.Message;

        var snapshot = new AuctionSnapshot
        {
            Id = message.Id,
            ReservePrice = message.ReservePrice,
            AuctionEnd = context.Message.AuctionEnd,
            Status = AuctionStatus.Live
        };

        _context.AuctionSnapshots.Add(snapshot);
        await _context.SaveChangesAsync();
    }
}
