using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionController: ControllerBase
{

    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;
    private readonly SearchSyncService _searchSync;
    public AuctionController(AuctionDbContext context, IMapper mapper, SearchSyncService searchSync)
    {
        _context = context;
        _mapper = mapper;
        _searchSync = searchSync;
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
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        if(auction == null)return NotFound();
        return _mapper.Map<AuctionDto>(auction);
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
        var success = await _context.SaveChangesAsync() > 0;

        if (!success) return BadRequest("Could not save auction");
         // sync with Search service (Mongo)
        await _searchSync.SyncWithSearchService(auction);
        return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id },
            _mapper.Map<AuctionDto>(auction));
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

        auction.UpdatedAt = DateTime.UtcNow;

        var success = await _context.SaveChangesAsync() > 0;
        await _searchSync.SyncWithSearchService(auction);
        if(!success) return BadRequest("Could not save changes");
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

        return Ok("Auction deleted successfully");
        
    }
    
}