using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AuctionService.Data;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Services
{
    public class OutboxPublisher: BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxPublisher> _logger;

    public OutboxPublisher(IServiceProvider serviceProvider, ILogger<OutboxPublisher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
            var publisher = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            var pending = await db.OutboxMessages
                .Where(x => x.ProcessedOn == null)
                .OrderBy(x => x.OccurredOn)
                .Take(50)
                .ToListAsync(stoppingToken);

            foreach (var msg in pending)
            {
                try
                {
                    object? deserialized = msg.Type switch
                    {
                        nameof(AuctionCreated) => JsonSerializer.Deserialize<AuctionCreated>(msg.Content),
                        nameof(AuctionUpdated) => JsonSerializer.Deserialize<AuctionUpdated>(msg.Content),
                        nameof(AuctionDeleted) => JsonSerializer.Deserialize<AuctionDeleted>(msg.Content),
                        _ => null
                    };

                    if (deserialized == null)
                    {
                        msg.ProcessedOn = DateTime.UtcNow;
                        continue;
                    }

                    await publisher.Publish(deserialized, stoppingToken);

                    msg.ProcessedOn = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publishing outbox message {Id}", msg.Id);
                    // leave ProcessedOn null – will be retried next loop
                }
            }

            await db.SaveChangesAsync(stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}

}