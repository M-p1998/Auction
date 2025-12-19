using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuctionService.Controllers;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.UnitTests.Helpers;
using AuctionService.UnitTests.Mapping;
using AutoMapper;
using Contracts.Enums;
using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;

namespace AuctionService.UnitTests.Controllers
{
    public class AuctionControllerTests
    {
        private readonly AuctionDbContext _db;
        private readonly IMapper _mapper;
        private readonly Mock<IDistributedCache> _cache = new();
        private readonly Mock<IPublishEndpoint> _publishEndpoint = new();

        public AuctionControllerTests()
        {
                var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AuctionMappingProfile>();
            });

            _mapper = mapperConfig.CreateMapper();

            // DbContext setup (usually InMemory)
            var options = new DbContextOptionsBuilder<AuctionDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _db = new AuctionDbContext(options);

            // mock Redis cache
            _cache = new Mock<IDistributedCache>();

            _cache
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

             // Mock MassTransit publish endpoint
            _publishEndpoint = new Mock<IPublishEndpoint>();

            _publishEndpoint
                .Setup(p => p.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        private AuctionController CreateController(bool isAdmin = true)
        {
            var controller = new AuctionController(
                _db,
                _mapper,
                _publishEndpoint.Object,
                _cache.Object
            );

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, "admin@auction.com"),
                new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User")
            };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"))
                }
            };

            return controller;
        }

        [Fact]
        public async Task CreateAuction_Should_Create_Auction()
        {
            var controller = CreateController();
             // fake user
            controller.ControllerContext = FakeAuthenticatedContext();

            var dto = new CreateAuctionDto
            {
                Make = "Toyota",
                Model = "Camry",
                Year = 2020,
                Color = "Black",
                Mileage = 20000,
                ImageUrl = "img.jpg",
                ReservePrice = 10000,
                AuctionEnd = DateTime.UtcNow.AddDays(7)
            };

            // Act
            var result = await controller.CreateAuction(dto);

            // Assert
            var created = result.Result as CreatedAtActionResult;
            created.Should().NotBeNull();

            var auction = await _db.Auctions.FirstOrDefaultAsync();
            auction.Should().NotBeNull();
            auction!.Item.Make.Should().Be("Toyota");
        }

        [Fact]
        public async Task CreateAuction_Should_Fail_When_Model_Invalid()
        {
            // Arrange
            var controller = CreateController();
            controller.ControllerContext = FakeAuthenticatedContext();

            var dto = new CreateAuctionDto();
            controller.ModelState.AddModelError("Make", "Required");

            // Act
            var result = await controller.CreateAuction(dto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetAuctionById_Should_Return_NotFound()
        {
            var controller = CreateController();
            controller.ControllerContext = FakeAuthenticatedContext();

            var result = await controller.GetAuctionById(Guid.NewGuid());

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task UpdateAuction_Should_Update_Auction()
        {
            var controller = CreateController(isAdmin: true);
            var auction = new Auction
            {
                Id = Guid.NewGuid(),
                Seller = "admin@auction.com",
                Status = AuctionStatus.Live,
                Item = new Item
                {
                    Make = "Old",
                    Model = "Old",
                    Year = 2010
                }
            };

            _db.Auctions.Add(auction);
            await _db.SaveChangesAsync();


            controller.ControllerContext = FakeAuthenticatedContext("admin@auction.com", isAdmin: true);

            var dto = new UpdateAuctionDto
            {
                Make = "New"
            };

            var result = await controller.UpdateAuction(auction.Id, dto);

            result.Should().BeOfType<OkObjectResult>();
            _db.Auctions.First().Item.Make.Should().Be("New");
        }

        [Fact]
        public async Task DeleteAuction_Should_Fail_When_Live()
        {
            var auction = new Auction
            {
                Id = Guid.NewGuid(),
                Status = AuctionStatus.Live,
                Item = new Item()
            };

            _db.Auctions.Add(auction);
            await _db.SaveChangesAsync();

            var controller = CreateController();
            controller.ControllerContext = FakeAuthenticatedContext();

            var result = await controller.DeleteAuction(auction.Id);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        private static ControllerContext FakeAuthenticatedContext(string email = "admin@auction.com",
        bool isAdmin = true)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User")
            };

            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
                }
            };
        }

    }


}
