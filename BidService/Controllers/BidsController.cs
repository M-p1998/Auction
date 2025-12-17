using BidService.Data;
using BidService.Entities;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BidService.DTO;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace BidService.Controllers;

[ApiController]
[Route("api/bids")]
[Authorize(Roles="User")]
public class BidsController : ControllerBase
{
    private readonly BidDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    // private readonly IHttpClientFactory _httpClientFactory;

    public BidsController(BidDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        // _httpClientFactory = httpClientFactory;
    }

    [Authorize(Roles = "User")]
    [HttpPost]
    public async Task<IActionResult> PlaceBid([FromBody]CreateBidRequest request)
    {
        // var bidder = User.Identity?.Name ?? "anonymous";
        // var bidderEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        // // var bidderEmail = User.FindFirstValue(ClaimTypes.Email);

        // // var bid = new Bid
        // // {
        // //     AuctionId = request.AuctionId,
        // //     Amount = request.Amount,
        // //     Bidder = bidder
        // // };

        // // _context.Bids.Add(bid);
        // // await _context.SaveChangesAsync();

        // await _publishEndpoint.Publish(new BidPlaced
        // {
        //     AuctionId = request.AuctionId,
        //     Bidder = bidderEmail,
        //     Amount = request.Amount
        // });

        // // return Ok("Bid placed successfully");
        // // var bidder = User.Identity?.Name ?? "unknown-user";

        // var bidderEmail = User.FindFirstValue(ClaimTypes.Email);
        // if (string.IsNullOrEmpty(bidderEmail))
        //     return Unauthorized("Invalid token");
        
        // var client = _httpClientFactory.CreateClient("auction");
        // var auction = await client.GetFromJsonAsync<AuctionSummaryDto>($"api/auctions/{request.AuctionId}");

        // if (auction == null) return BadRequest("Auction not found");

        // if (request.Amount < auction.ReservePrice)
        // {
        //     return BadRequest("Bid must be greater than or equal to reserve price");
        // }

        // var auction = await _context.Auctions.FirstOrDefaultAsync(x => x.Id == request.AuctionId);

        // if (auction == null)
        //     return BadRequest("Auction not found");

        // if (auction.Status != "Live")
        //     return BadRequest("Auction has ended");

        // if (request.Amount < auction.ReservePrice)
        //     return BadRequest("Bid must be greater than or equal to reserve price");
        // var bid = new Bid
        // {
        //     Id = Guid.NewGuid(),
        //     AuctionId = request.AuctionId,
        //     Bidder = bidderEmail,
        //     Amount = request.Amount
        // };

        // _context.Bids.Add(bid);
        // await _context.SaveChangesAsync();

        // await _publishEndpoint.Publish(new BidPlaced
        // {
        //     AuctionId = bid.AuctionId,
        //     Bidder = bid.Bidder,
        //     Amount = bid.Amount
        // });

        // return Ok(bid);

    var bidderEmail = User.FindFirstValue(ClaimTypes.Email);
    if (string.IsNullOrEmpty(bidderEmail))
        return Unauthorized("Invalid token");

    var auctionSnapshot = await _context.AuctionSnapshots
        .FirstOrDefaultAsync(a => a.Id == request.AuctionId);

    if (auctionSnapshot == null)
        return NotFound("Auction not found");

    // ✅ Reserve price validation
    if (request.Amount < auctionSnapshot.ReservePrice)
        return BadRequest("Bid must be greater than or equal to reserve price");

    var bid = new Bid
    {
        Id = Guid.NewGuid(),
        AuctionId = request.AuctionId,
        Bidder = bidderEmail,
        Amount = request.Amount
    };

    _context.Bids.Add(bid);
    await _context.SaveChangesAsync();

    await _publishEndpoint.Publish(new BidPlaced
    {
        AuctionId = bid.AuctionId,
        Bidder = bid.Bidder,
        Amount = bid.Amount
    });
    return Ok(bid);
    }

    [HttpGet("{auctionId}")]
    public async Task<IActionResult> GetBids(Guid auctionId)
    {
        var bids = _context.Bids
            .Where(x => x.AuctionId == auctionId)
            .OrderByDescending(x => x.Amount)
            .ToListAsync();

        return Ok(bids);
    }
}
