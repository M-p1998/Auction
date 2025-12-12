using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
    {
        private readonly IMapper _mapper;

        public AuctionUpdatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        Console.WriteLine("---> Consuming AuctionUpdated");

        var message = context.Message;

        var item = await DB.Find<Item>()
            .Match(i => i.AuctionId == message.Id)
            .ExecuteFirstAsync();

        // if (item == null)
        // {
        //     // optional: create if missing
        //     item = _mapper.Map<Item>(message);
        // }
        // else
        // {
            item.Make = message.Make;
            item.Model = message.Model;
            item.Color = message.Color;
            item.Year = message.Year;
            item.Mileage = message.Mileage;
            // item.ImageUrl = message.ImageUrl;
            // item.ReservePrice = message.ReservePrice;
            // item.AuctionEnd = message.AuctionEnd;
        // }

        await item.SaveAsync();
    }
    }
}