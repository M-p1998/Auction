using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

    [ApiController]
    [Route("api/auth")]
    public class AuthController: ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IConfiguration _config;
        public AuthController(AuctionDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
            .Select(u => new { u.Id, u.Email })
            .ToListAsync();

            return Ok(users);
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser(RegisterDto request)
        {
            var exists = await _context.Users.AnyAsync(a => a.Email == request.Email);
            if (exists){
                return BadRequest("User already exists");
            }
            // plain text
            // var user = new User{
            //     Email = request.Email,
            //     Password = request.Password
            // };
            var hashedPassword = PasswordHasher.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                Password = hashedPassword // Store hashed password
            };
            _context.Users.Add(user);
            var result = await _context.SaveChangesAsync() > 0;
            if (!result) return BadRequest("Could not register user");
            return Ok("User registered successfully");
                
        }

        // [HttpPost("login-user")]
        // public async Task<IActionResult> LoginUser(LoginDto request)
        // {
        //     var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);

        //     if (user == null)
        //     {
        //         return Unauthorized("Invalid email or password");
        //     }
        //     return Ok("User logged in successfully");
        // }

        // [HttpGet("admins")]
        // public async Task<IActionResult> GetAdmins()
        // {
        //     var admins = await _context.Admins
        //     .Select(a => new { a.Id, a.Email })
        //     .ToListAsync();

        //     return Ok(admins);
        // }

        [HttpPost("login-user")]
        public async Task<IActionResult> LoginUser(LoginDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized("User not found!");

            if (!PasswordHasher.VerifyPassword(request.Password, user.Password))
                return Unauthorized("Invalid password");

            var token = JwtHelper.GenerateToken(
                user.Email,
                "User",              // role
                _config
            );

            return Ok(new
            {
                message = "User logged in successfully",
                token = token
            });
        }

        [HttpPost("login-admin")]
        public async Task<IActionResult> LoginAdmin(LoginDto request)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == request.Email);

            if(admin == null)
            {
                return Unauthorized("Admin not found");
            }
            if(!PasswordHasher.VerifyPassword(request.Password, admin.PasswordHash))
            {
                return Unauthorized("Invalid password");
            }
            var token = JwtHelper.GenerateToken(admin.Email, "Admin", _config);
            Console.WriteLine("Generated Token: " + token);
            return Ok(new
            {
                message = "Admin logged in successfully",
                token = token
            });
        }
        }
