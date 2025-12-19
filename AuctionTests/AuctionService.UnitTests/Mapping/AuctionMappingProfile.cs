using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;

namespace AuctionService.UnitTests.Mapping
{
    public class AuctionMappingProfile : Profile
    {
        public AuctionMappingProfile()
        {
            CreateMap<Auction, AuctionDto>();
            CreateMap<CreateAuctionDto, Auction>();
            CreateMap<UpdateAuctionDto, Auction>();
        }
    }
}