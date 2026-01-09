using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Application.DTOs.Request;
using ShoppingCart.Application.DTOs.Response;
using ShoppingCart.EntityCore.Data;
using ShoppingCart.Services;
using ShoppingCart.EntityCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCart.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class AuthController : ControllerBase
        {
            private readonly AppDbContext _context;
            private readonly JwtService _jwtService;
            private readonly PasswordHasher<User> _passwordHasher;

            public AuthController(AppDbContext context, JwtService jwtService)
            {
                _context = context;
                _jwtService = jwtService;
                _passwordHasher = new PasswordHasher<User>();
            }

            [HttpPost("login")]
            public async Task<IActionResult> Login(LoginRequestDTO loginDto)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                if (user == null)
                    return Unauthorized(new { message = "Invalid credentials" });


                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
                if (result == PasswordVerificationResult.Failed)
                    return Unauthorized(new { message = "Invalid credentials" });

                var token = _jwtService.GenerateToken(user);
                return Ok(new LoginResponseDTO { Token = token, Email = user.Email });
            }
        }
    }
