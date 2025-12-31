using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;

// using AuctionService.Services;

// using AuctionService.Services;

// using AuctionService.Services;
using AutoMapper;
using Contracts;
using Contracts.Enums;
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
            .OrderByDescending(x => x.CreatedAt)
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var auction = new Auction
        {
            Id = Guid.NewGuid(),
            ReservePrice = dto.ReservePrice,
            AuctionEnd = dto.AuctionEnd.ToUniversalTime(),
            Status = AuctionStatus.Live,
            Seller = User.FindFirstValue(ClaimTypes.Email),
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
        // if (auction.Seller != User.Identity.Name)
        //     return Unauthorized("You cannot update another admin’s auction");

        if (auction.AuctionEnd <= DateTime.UtcNow)
            return BadRequest("Ended auctions cannot be updated");
        
        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;
        auction.Item.ImageUrl = updateAuctionDto.ImageUrl ?? auction.Item.ImageUrl;
        // auction.AuctionEnd  = updateAuctionDto.AuctionEnd ?? auction.AuctionEnd;
        if (updateAuctionDto.AuctionEnd.HasValue)
        {
            auction.AuctionEnd = updateAuctionDto.AuctionEnd.Value.ToUniversalTime();
        }
        if (updateAuctionDto.ReservePrice.HasValue)
        {
            auction.ReservePrice = updateAuctionDto.ReservePrice.Value;
        }
        

        // auction.ReservePrice= updateAuctionDto.ReservePrice ?? auction.ReservePrice;
        // 🔴 ONLY update reserve price if provided
    // if (updateAuctionDto.ReservePrice.HasValue)
    // {
    //     auction.ReservePrice = updateAuctionDto.ReservePrice.Value;

    //     // Recalculate high bid ONLY if needed
    //     var validHighBid = await _context.Bids
    //         .Where(b => b.AuctionId == auction.Id &&
    //                     b.Amount >= auction.ReservePrice)
    //         .OrderByDescending(b => b.Amount)
    //         .FirstOrDefaultAsync();

    //     auction.CurrentHighBid = validHighBid?.Amount ?? 0;
    //     auction.WinningBidder = validHighBid?.Bidder;
    // }

         // 🚨 IMPORTANT: track if reserve price changed
            // 🔴 ONLY handle reserve price IF provided
        // ✅ ONLY update reserve price if user sent it
    // if (updateAuctionDto.ReservePrice.HasValue)
    // {
    //     auction.ReservePrice = updateAuctionDto.ReservePrice.Value;

    //     // ✅ only wipe if currentHighBid is now invalid
    //     if (auction.CurrentHighBid < auction.ReservePrice)
    //     {
    //         auction.CurrentHighBid = 0;
    //         auction.WinningBidder = null;
    //     }
    // }
        // SAFETY CHECK: preserve valid high bid
        // if (auction.CurrentHighBid < auction.ReservePrice)
        // {
        //     auction.CurrentHighBid = 0;
        //     auction.WinningBidder = null;
        // }
        // Recalculate valid high bid
        // var validHighBid = await _context.Bids
        //     .Where(b => b.AuctionId == auction.Id &&
        //                 b.Amount >= auction.ReservePrice)
        //     .OrderByDescending(b => b.Amount)
        //     .Select(b => (decimal?)b.Amount)
        //     .FirstOrDefaultAsync();

        // auction.CurrentHighBid = validHighBid ?? 0;
        // auction.Winner = validHighBid == null ? null : auction.Winner;
        // var validHighBid = await _context.Bids
        //     .Where(b => b.AuctionId == auction.Id &&
        //                 b.Amount >= auction.ReservePrice)
        //     .OrderByDescending(b => b.Amount)
        //     .Select(b => (decimal?)b.Amount)
        //     .FirstOrDefaultAsync();

        // auction.CurrentHighBid = validHighBid ?? 0;

        // auction.WinningBidder = validHighBid == null
        //     ? null
        //     : auction.WinningBidder;

    //     var validHighBid = await _context.Bids
    //     .Where(b =>
    //         b.AuctionId == auction.Id &&
    //         b.Amount >= auction.ReservePrice)
    //     .OrderByDescending(b => b.Amount)
    //     .Select(b => new
    //     {
    //         b.Amount,
    //         b.Bidder
    //     })
    //     .FirstOrDefaultAsync();

    // auction.CurrentHighBid = validHighBid?.Amount ?? 0;
    // auction.WinningBidder = validHighBid?.Bidder;

    // var validHighBid = await _context.Bids
    //     .Where(b =>
    //         b.AuctionId == auction.Id &&
    //         b.Amount >= auction.ReservePrice)
    //     .OrderByDescending(b => b.Amount)
    //     .Select(b => (decimal?)b.Amount)
    //     .FirstOrDefaultAsync();

    // if (validHighBid.HasValue)
    // {
    //     auction.CurrentHighBid = validHighBid.Value;
    // }
    // else
    // {
    //     auction.CurrentHighBid = 0;
    //     auction.WinningBidder = null;
    // }

        auction.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        // var success = await _context.SaveChangesAsync() > 0;
        // if (!success) return BadRequest("Could not save changes");


        // var success = await _context.SaveChangesAsync() > 0;
        // // await _searchSync.SyncWithSearchService(auction);
        // if(!success) return BadRequest("Could not save changes");
        // Publish AuctionUpdated event
        var updatedEvent = new AuctionUpdated
        {
            Id = auction.Id,
            Make = auction.Item.Make,
            Model = auction.Item.Model,
            Year = auction.Item.Year,
            Color = auction.Item.Color,
            Mileage = auction.Item.Mileage,
            AuctionEnd   = auction.AuctionEnd,
            ReservePrice = auction.ReservePrice,
            CurrentHighBid = auction.CurrentHighBid,
            ImageUrl = auction.Item.ImageUrl
        };
        // var updateEvent = new AuctionUpdated
        // {
        //     Id = auction.Id,
        //     ReservePrice = auction.ReservePrice,
        //     AuctionEnd = auction.AuctionEnd,
        //     ImageUrl = auction.Item.ImageUrl
        // };
        
        var outbox = new OutboxMessage
        {
             Id = Guid.NewGuid(),
             Type = nameof(AuctionUpdated),
             Content = JsonSerializer.Serialize(updatedEvent),
         };
        _context.OutboxMessages.Add(outbox);
        // var success = await _context.SaveChangesAsync() > 0;
        // if (!success)
        //     return BadRequest("Could not save changes");
        await _context.SaveChangesAsync();
        await _cache.RemoveAsync($"auction:{id}");

        // await _publishEndpoint.Publish(updatedEvent);
        return Ok("Auction updated successfully");
    }

// [Authorize(Roles = "Admin")]
// [HttpPut("{id}")]
// public async Task<IActionResult> UpdateAuction(Guid id, UpdateAuctionDto dto)
// {
//     var auction = await _context.Auctions
//         .Include(x => x.Item)
//         .FirstOrDefaultAsync(x => x.Id == id);

//     if (auction == null) return NotFound();

//     if (auction.AuctionEnd <= DateTime.UtcNow)
//         return BadRequest("Ended auctions cannot be updated");

//     // Update item fields
//     auction.Item.Make = dto.Make ?? auction.Item.Make;
//     auction.Item.Model = dto.Model ?? auction.Item.Model;
//     auction.Item.Color = dto.Color ?? auction.Item.Color;
//     auction.Item.Mileage = dto.Mileage ?? auction.Item.Mileage;
//     auction.Item.Year = dto.Year ?? auction.Item.Year;
//     auction.Item.ImageUrl = dto.ImageUrl ?? auction.Item.ImageUrl;

//     // Update auction fields
//     if (dto.AuctionEnd.HasValue)
//         auction.AuctionEnd = dto.AuctionEnd.Value.ToUniversalTime();

//     auction.ReservePrice = dto.ReservePrice ?? auction.ReservePrice;

//     // ❗ DO NOT TOUCH CurrentHighBid here
//     // ❗ DO NOT TOUCH WinningBidder here

//     auction.UpdatedAt = DateTime.UtcNow;

//     await _context.SaveChangesAsync();

//     // Publish event (read-only snapshot)
//     var updatedEvent = new AuctionUpdated
//     {
//         Id = auction.Id,
//         Make = auction.Item.Make,
//         Model = auction.Item.Model,
//         Year = auction.Item.Year,
//         Color = auction.Item.Color,
//         Mileage = auction.Item.Mileage,
//         AuctionEnd = auction.AuctionEnd,
//         ReservePrice = auction.ReservePrice,
//         CurrentHighBid = auction.CurrentHighBid,
//         ImageUrl = auction.Item.ImageUrl
//     };

//     _context.OutboxMessages.Add(new OutboxMessage
//     {
//         Id = Guid.NewGuid(),
//         Type = nameof(AuctionUpdated),
//         Content = JsonSerializer.Serialize(updatedEvent)
//     });

//     await _context.SaveChangesAsync();
//     await _cache.RemoveAsync($"auction:{id}");

//     return Ok("Auction updated successfully");
// }


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
        // if (auction.Seller != User.Identity.Name)
        //     return Unauthorized("You cannot delete another admin’s auction");

        // _context.Auctions.Remove(auction);
        // var success = await _context.SaveChangesAsync() > 0;

        // if (!success)
        //     return BadRequest("Failed to delete auction");

        //  // Publish AuctionDeleted event
        // // await _publishEndpoint.Publish(new AuctionDeleted { Id = id });
        // var deleteEvent = new AuctionDeleted
        // {
        //     Id = auction.Id
        // };
        // var outbox = new OutboxMessage
        // {
        //     Id = Guid.NewGuid(),
        //     Type = nameof(AuctionDeleted),
        //     Content = JsonSerializer.Serialize(deleteEvent),
        // };

        // _context.OutboxMessages.Add(outbox);
        // await _context.SaveChangesAsync();
        // await _cache.RemoveAsync($"auction:{id}");

        // return Ok("Auction deleted successfully");
        if (auction.Status == AuctionStatus.Live)
        return BadRequest("Cannot delete a live auction");

        _context.Auctions.Remove(auction);

        var deleteEvent = new AuctionDeleted { Id = auction.Id };

        _context.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(AuctionDeleted),
            Content = JsonSerializer.Serialize(deleteEvent)
        });

        await _context.SaveChangesAsync();
        await _cache.RemoveAsync($"auction:{auction.Id}");

        return Ok("Auction deleted successfully");
        
    }
    
}