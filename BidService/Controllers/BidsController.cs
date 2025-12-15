using BidService.Data;
using BidService.Entities;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BidService.DTO;
using Contracts;
using System.Security.Claims;


namespace BidService.Controllers;

[ApiController]
[Route("api/bids")]
[Authorize(Roles="User")]
public class BidsController : ControllerBase
{
    private readonly BidDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public BidsController(BidDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    [Authorize(Roles = "User")]
    [HttpPost]
    public async Task<IActionResult> PlaceBid(CreateBidRequest request)
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

        // return Ok("Bid placed successfully");
        // var bidder = User.Identity?.Name ?? "unknown-user";
        var bidderEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(bidderEmail))
            return Unauthorized("Invalid token");

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
    public IActionResult GetBids(Guid auctionId)
    {
        var bids = _context.Bids
            .Where(x => x.AuctionId == auctionId)
            .OrderByDescending(x => x.Amount)
            .ToList();

        return Ok(bids);
    }
}
