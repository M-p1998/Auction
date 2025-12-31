// using Contracts;
// using MassTransit;
// using AuctionService.Data;
// using Microsoft.EntityFrameworkCore;
// using System.Threading.Tasks;
// using System;
// using Contracts.Enums;

// namespace AuctionService.Consumers;

// public class BidPlacedConsumer : IConsumer<BidPlaced>
// {
//     private readonly AuctionDbContext _context;

//     public BidPlacedConsumer(AuctionDbContext context)
//     {
//         _context = context;
//     }

//     // public async Task Consume(ConsumeContext<BidPlaced> context)
//     // {
//     //     var auction = await _context.Auctions
//     //         .FirstOrDefaultAsync(x => x.Id == context.Message.AuctionId);

//     //     if (auction == null) return;
//     //     // Ignore bids after auction end
//     //     // if (auction.Status != AuctionStatus.Live) return;

//     //     // IMPORTANT: ignore bids below reserve price
//     //     if (context.Message.Amount < auction.ReservePrice) return;

//     //     if (auction.AuctionEnd < DateTime.UtcNow)
//     //     {   
//     //         // Allow updating WinningBidder, but NOT changing status
//     //         if (context.Message.Amount > auction.CurrentHighBid)
//     //         {
//     //             auction.CurrentHighBid = context.Message.Amount;
//     //             auction.WinningBidder = context.Message.Bidder;
//     //             auction.UpdatedAt = DateTime.UtcNow;
//     //         }

//     //         await _context.SaveChangesAsync();
//     //         return;
//     //     }

//     //     if (context.Message.Amount > auction.CurrentHighBid)
//     //     {
//     //         auction.CurrentHighBid = context.Message.Amount;
//     //         auction.WinningBidder = context.Message.Bidder;
//     //         auction.UpdatedAt = DateTime.UtcNow;
//     //     }

//     //     await _context.SaveChangesAsync();
//     // }

//     public async Task Consume(ConsumeContext<BidPlaced> context)
//     {
//         var auction = await _context.Auctions
//             .FirstOrDefaultAsync(x => x.Id == context.Message.AuctionId);

//         if (auction == null) return;

//         // 1️⃣ Reject bids after auction ends
//         if (auction.AuctionEnd <= DateTime.UtcNow)
//             return;

//         // 2️⃣ Reject bids below reserve price
//         if (context.Message.Amount < auction.ReservePrice)
//             return;

//         // Ignore bids lower than current valid high bid
//         if (context.Message.Amount <= auction.CurrentHighBid)
//             return;

//         // 3️⃣ Accept only higher valid bids
//         // if (context.Message.Amount > auction.CurrentHighBid)
//         // {
//         //     auction.CurrentHighBid = context.Message.Amount;
//         //     auction.WinningBidder = context.Message.Bidder;
//         //     auction.UpdatedAt = DateTime.UtcNow;

//         //     await _context.SaveChangesAsync();
//         // }

//         // auction.CurrentHighBid = context.Message.Amount;
//         // auction.WinningBidder = context.Message.Bidder;
//         // auction.UpdatedAt = DateTime.UtcNow;

//         // await _context.SaveChangesAsync();

//         // // Trigger recalculation
//         //     await context.Publish(new HighBidRecalculated
//         //     {
//         //         AuctionId = auction.Id
//         //     });
//         auction.CurrentHighBid = context.Message.Amount;
//         auction.WinningBidder = context.Message.Bidder;
//         auction.UpdatedAt = DateTime.UtcNow;

//         await _context.SaveChangesAsync();
        
//     }


  
// }
