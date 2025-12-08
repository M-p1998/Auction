using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.DTOs;
using SearchService.Models;

namespace SearchService.Controllers
{
    [ApiController]
    [Route("api/search")]
    // public class SearchController: ControllerBase
    // {
    //     [HttpGet]
    //     public async Task<IActionResult> Search(string searchTerm)
    //     {
    //         if (string.IsNullOrWhiteSpace(searchTerm))
    //         {
    //             return BadRequest("Query parameter is required.");
    //         }

    //         // var query = DB.Find<Item>();
    //         // query.Sort(x => x.Ascending(a=> a.Make));
    //         // var result = await query.ExecuteAsync();

    //         // var result = await DB.Find<Item>()
    //         //     .Match(x => x.Make.Contains(searchTerm) || x.Model.Contains(searchTerm))
    //         //     .Sort(x => x.Ascending(a => a.Make))
    //         //     .ExecuteAsync();

    //         // return Ok(result);

    //         searchTerm = searchTerm.ToLower();
    //         var result = await DB.Find<Item>()
    //             .Match(item =>
    //                 item.Make.ToLower().Contains(searchTerm) ||
    //                 item.Model.ToLower().Contains(searchTerm)  ||
    //                 item.Color.ToLower().Contains(searchTerm)
    //             )
    //             .Sort(x => x.Ascending(a => a.Make))
    //             .ExecuteAsync();

    //         if (!result.Any())
    //             return NoContent();

    //         return Ok(result);
    //     }
    // } 



    public class AuctionWebhookController : ControllerBase
    {
        [HttpPost("upsert")]
        public async Task<IActionResult> UpsertAuction(AuctionDTO auction)
        {
            var item = await DB.Find<Item>()
                .Match(x => x.ImageUrl == auction.ImageUrl)
                .ExecuteFirstAsync();

            if (item == null)
            {
                item = new Item();
            }

            item.Make = auction.Make;
            item.Model = auction.Model;
            item.Color = auction.Color;
            item.Year = auction.Year;
            item.Mileage = auction.Mileage;
            item.ImageUrl = auction.ImageUrl;
            item.AuctionEnd = auction.AuctionEnd;
            item.ReservePrice = auction.ReservePrice;

            await item.SaveAsync();

            return Ok("Auction synced to Search DB");
        }


        [HttpGet]
        public async Task<IActionResult> Search([FromQuery]string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Query parameter is required.");

            searchTerm = searchTerm.ToLower();

            var result = await DB.Find<Item>()
                .Match(i =>
                    i.Make.ToLower().Contains(searchTerm) ||
                    i.Model.ToLower().Contains(searchTerm) ||
                    i.Color.ToLower().Contains(searchTerm)
                )
                .Sort(i => i.Ascending(x => x.Make))
                .ExecuteAsync();

            if (!result.Any()) 
                return NoContent();

            return Ok(result);
        }
        // public async Task<IActionResult> Search(string searchTerm)
        // {
        //     if (string.IsNullOrWhiteSpace(searchTerm))
        //     {
        //         return BadRequest("Query parameter is required.");
        //     }

        //     // var query = DB.Find<Item>();
        //     // query.Sort(x => x.Ascending(a=> a.Make));
        //     // var result = await query.ExecuteAsync();

        //     // var result = await DB.Find<Item>()
        //     //     .Match(x => x.Make.Contains(searchTerm) || x.Model.Contains(searchTerm))
        //     //     .Sort(x => x.Ascending(a => a.Make))
        //     //     .ExecuteAsync();

        //     // return Ok(result);

        //     searchTerm = searchTerm.ToLower();
        //     var result = await DB.Find<Item>()
        //         .Match(item =>
        //             item.Make.ToLower().Contains(searchTerm) ||
        //             item.Model.ToLower().Contains(searchTerm)  ||
        //             item.Color.ToLower().Contains(searchTerm)
        //         )
        //         .Sort(x => x.Ascending(a => a.Make))
        //         .ExecuteAsync();

        //     if (!result.Any())
        //         return NoContent();

        //     return Ok(result);
        // }
    }

}