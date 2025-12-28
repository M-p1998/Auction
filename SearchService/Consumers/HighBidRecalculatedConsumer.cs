// using Contracts;
// using MassTransit;
// using MongoDB.Driver;
// using SearchService.Models; // adjust to your model namespace

// namespace SearchService.Consumers;

// public class HighBidRecalculatedConsumer : IConsumer<HighBidRecalculated>
// {
//     private readonly IMongoCollection<AuctionItem> _collection;

//     public HighBidRecalculatedConsumer(IMongoDatabase db)
//     {
//         _collection = db.GetCollection<AuctionItem>("Auctions");
//     }

//     public async Task Consume(ConsumeContext<HighBidRecalculated> context)
//     {
//         var filter = Builders<AuctionItem>.Filter.Eq(x => x.Id, context.Message.AuctionId.ToString());
//         var update = Builders<AuctionItem>.Update
//             .Set(x => x.CurrentHighBid, context.Message.CurrentHighBid);

//         await _collection.UpdateOneAsync(filter, update);
//     }
// }
