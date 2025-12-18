using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly IHubContext<NotificationHub> _hub;

    public BidPlacedConsumer(IHubContext<NotificationHub> hub)
    {
        _hub = hub;
    }

    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine($" BidPlaced received for {context.Message.AuctionId}");

        await _hub.Clients
            .Group(context.Message.AuctionId.ToString())
            .SendAsync("BidPlaced", context.Message);
    }
}
