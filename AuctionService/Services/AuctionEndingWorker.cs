using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuctionService.Data;
using Contracts.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class AuctionEndingWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AuctionEndingWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();

            var now = DateTime.UtcNow;

            var auctionsToEnd = await db.Auctions
                .Where(a =>
                    a.Status == AuctionStatus.Live &&
                    a.AuctionEnd <= now
                    // a.AuctionEnd <= now.AddSeconds(-5)
                )
                .ToListAsync(stoppingToken);

            foreach (var auction in auctionsToEnd)
            {
                if (auction.CurrentHighBid >= auction.ReservePrice)
                {
                    auction.Status = AuctionStatus.Ended;
                    auction.SoldAmount = auction.CurrentHighBid;
                    auction.Winner = auction.WinningBidder;
                }
                else
                {
                    auction.Status = AuctionStatus.ReserveNotMet;
                    auction.SoldAmount = 0;
                    auction.Winner = auction.WinningBidder;
                }

                auction.UpdatedAt = DateTime.UtcNow;
            }

            await db.SaveChangesAsync(stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
