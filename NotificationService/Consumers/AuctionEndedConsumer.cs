using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

public class AuctionEndedConsumer : IConsumer<AuctionEnded>
{
    private readonly IHubContext<NotificationHub> _hub;

    public AuctionEndedConsumer(IHubContext<NotificationHub> hub)
    {
        _hub = hub;
    }

    public async Task Consume(ConsumeContext<AuctionEnded> context)
    {
        Console.WriteLine($"AuctionEnded received for {context.Message.Id}");

        await _hub.Clients
            .Group(context.Message.Id.ToString())
            .SendAsync("AuctionEnded", context.Message);
    }
}
