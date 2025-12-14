using System.Text.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
// using AuctionService.Services;

// using AuctionService.Services;

// using AuctionService.Services;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionController: ControllerBase
{

    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;
    // private readonly SearchSyncService _searchSync;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IDistributedCache _cache;
    public AuctionController(AuctionDbContext context, IMapper mapper,  IPublishEndpoint publishEndpoint, IDistributedCache cache)
    {
        _context = context;
        _mapper = mapper;
        // _searchSync = searchSync;
        _publishEndpoint = publishEndpoint;
        _cache = cache;
    }
   

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
    {
        var auctions = await _context.Auctions
            .Include(x => x.Item)
            .OrderBy(x => x.Item.Make)
            .ToListAsync();
        return _mapper.Map<List<AuctionDto>>(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        // var auction = await _context.Auctions
        //     .Include(x => x.Item)
        //     .FirstOrDefaultAsync(x => x.Id == id);
        // if(auction == null)return NotFound();
        // return _mapper.Map<AuctionDto>(auction);

        var cacheKey = $"auction:{id}";

        // Try cache
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            var dto = JsonSerializer.Deserialize<AuctionDto>(cached);
            return Ok(dto);
        }

        // Fallback to DB
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null) return NotFound();

        var result = _mapper.Map<AuctionDto>(auction);

        // Save to cache
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(result),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

        return Ok(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto dto)
    {
        var auction = new Auction
        {
            Id = Guid.NewGuid(),
            ReservePrice = dto.ReservePrice,
            AuctionEnd = dto.AuctionEnd,
            Status = Status.Live,
            Seller = User.Identity.Name,
            Item = new Item
            {
                Id = Guid.NewGuid(),
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                Color = dto.Color,
                Mileage = dto.Mileage,
                ImageUrl = dto.ImageUrl
            }
        };
        // auction.Seller = "admin";
        _context.Auctions.Add(auction);
        // var success = await _context.SaveChangesAsync() > 0;

        // var newAuction = _mapper.Map<Auction>(auction);
        // publish event to RabbitMQ
        // await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

        var createdEvent = new AuctionCreated
        {
            Id = auction.Id,
            AuctionEnd = auction.AuctionEnd,
            ReservePrice = auction.ReservePrice,
            Make = auction.Item.Make,
            Model = auction.Item.Model,
            Year = auction.Item.Year,
            Mileage = auction.Item.Mileage,
            Color = auction.Item.Color,
            ImageUrl = auction.Item.ImageUrl
        };
        // await _publishEndpoint.Publish(createdEvent);

        // var outbox = new OutboxMessage
        // {
        //     Id = Guid.NewGuid(),
        //     Type = nameof(AuctionCreated),
        //     Content = JsonSerializer.Serialize(createdEvent),
        // };

        // Save event to OUTBOX ONLY
        _context.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(AuctionCreated),
            Content = JsonSerializer.Serialize(createdEvent),

        });

        var success = await _context.SaveChangesAsync() > 0;
        // _context.OutboxMessages.Add(outbox);
        // await _context.SaveChangesAsync();
        if (!success) return BadRequest("Could not save auction");
        var newAuction = _mapper.Map<AuctionDto>(auction);
         // sync with Search service (MongoDB)
        // await _searchSync.SyncWithSearchService(auction);
        return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id },
            newAuction);
    }



    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await _context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        if(auction == null)return NotFound();

         // Only the admin can update it
        if (auction.Seller != User.Identity.Name)
            return Unauthorized("You cannot update another admin’s auction");
        
        auction.Item.Make = updateAuctionDto.Make ??  auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ??  auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ??  auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ??  auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ??  auction.Item.Year;
        // auction.AuctionEnd  = updateAuctionDto.AuctionEnd; ?? auction.Item.AuctionEnd;
        // auction.ReservePrice= updateAuctionDto.ReservePrice ?? auction.ReservePrice;
        auction.UpdatedAt = DateTime.UtcNow;

        var success = await _context.SaveChangesAsync() > 0;
        // await _searchSync.SyncWithSearchService(auction);
        if(!success) return BadRequest("Could not save changes");
        // Publish AuctionUpdated event
        var updatedEvent = new AuctionUpdated
        {
            Id = auction.Id,
            Make = auction.Item.Make,
            Model = auction.Item.Model,
            Year = auction.Item.Year,
            Color = auction.Item.Color,
            Mileage = auction.Item.Mileage
            // AuctionEnd   = auction.AuctionEnd,
            // ReservePrice = auction.ReservePrice
        };
         var outbox = new OutboxMessage
         {
             Id = Guid.NewGuid(),
             Type = nameof(AuctionUpdated),
             Content = JsonSerializer.Serialize(updatedEvent),
         };
        _context.OutboxMessages.Add(outbox);
        await _context.SaveChangesAsync();
        await _cache.RemoveAsync($"auction:{id}");

        // await _publishEndpoint.Publish(updatedEvent);
        return Ok("Auction updated successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions
        .Include(a => a.Item)
        .FirstOrDefaultAsync(a => a.Id == id);

        if (auction == null)
            return NotFound();

        // Only the admin can delete it
        if (auction.Seller != User.Identity.Name)
            return Unauthorized("You cannot delete another admin’s auction");

        _context.Auctions.Remove(auction);
        var success = await _context.SaveChangesAsync() > 0;

        if (!success)
            return BadRequest("Failed to delete auction");

         // Publish AuctionDeleted event
        // await _publishEndpoint.Publish(new AuctionDeleted { Id = id });
        var deleteEvent = new AuctionDeleted
        {
            Id = auction.Id
        };
        var outbox = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(AuctionDeleted),
            Content = JsonSerializer.Serialize(deleteEvent),
        };

        _context.OutboxMessages.Add(outbox);
        await _context.SaveChangesAsync();
        await _cache.RemoveAsync($"auction:{id}");

        return Ok("Auction deleted successfully");
        
    }
    
}