
using System;
using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;


namespace SearchService.Consumers
{
    public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        private readonly IMapper _mapper;
        public AuctionCreatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Console.WriteLine("----> Consuming auction created:");

            // var item = _mapper.Map<Item>(context.Message);
            var msg = context.Message;

            var item = new Item
            {
                ID = msg.Id.ToString(), // MongoDB ID
                AuctionId = msg.Id,
                Make = msg.Make,
                Model = msg.Model,
                Color = msg.Color,
                Mileage = msg.Mileage,
                Year = msg.Year,
                ImageUrl = msg.ImageUrl,
                ReservePrice = msg.ReservePrice,
                AuctionEnd = msg.AuctionEnd
            };

            await item.SaveAsync();
        }
    }
}