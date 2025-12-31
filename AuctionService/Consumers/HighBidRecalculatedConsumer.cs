// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using AuctionService.Data;
// using Contracts;
// using MassTransit;
// using Microsoft.EntityFrameworkCore;

// namespace AuctionService.Consumers
// {
//     public class HighBidRecalculatedConsumer : IConsumer<HighBidRecalculated>
// {
//     private readonly AuctionDbContext _context;

//     public HighBidRecalculatedConsumer(AuctionDbContext context)
//     {
//         _context = context;
//     }

//     public async Task Consume(ConsumeContext<HighBidRecalculated> context)
//     {
//         var auction = await _context.Auctions
//             .FirstOrDefaultAsync(a => a.Id == context.Message.AuctionId);

//         if (auction == null) return;

//         // auction.CurrentHighBid = context.Message.CurrentHighBid;
//         // // auction.Winner = context.Message.CurrentHighBid == 0
//         // //     ? null
//         // //     : auction.Winner;
        
//         // // auction.UpdatedAt = DateTime.UtcNow;
//         // auction.CurrentHighBid = context.Message.CurrentHighBid;

//         // if (context.Message.CurrentHighBid == 0)
//         // {
//         //     auction.Winner = null;
//         // }

//         var validHighBid = await _context.Bids
//         .Where(b => b.AuctionId == auction.Id &&
//                     b.Amount >= auction.ReservePrice)
//         .OrderByDescending(b => b.Amount)
//         .Select(b => (decimal?)b.Amount)
//         .FirstOrDefaultAsync();

//         auction.CurrentHighBid = validHighBid ?? 0;
//         auction.Winner = validHighBid == null ? null : auction.Winner;
//         auction.UpdatedAt = DateTime.UtcNow;

//         await _context.SaveChangesAsync();
//     }
// }

// }