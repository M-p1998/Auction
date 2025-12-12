using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;
using Contracts;

namespace SearchService.Consumers
{
    public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
    {
        public async Task Consume(ConsumeContext<AuctionDeleted> context)
        {
            Console.WriteLine("---> Consuming AuctionDeleted");
            await DB.DeleteAsync<Item>(context.Message.Id.ToString());
            Console.WriteLine($"Deleted {context.Message.Id} from MongoDB");
            // await DB.DeleteAsync<Item>(i => i.AuctionId == context.Message.Id);
        }
    }
}