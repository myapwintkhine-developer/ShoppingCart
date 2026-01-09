using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Application.DTOs.Request;
using ShoppingCart.Services;

namespace ShoppingCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        private string GetUserId() =>
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _cartService.GetOrCreateCartAsync(GetUserId());
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] CartItemRequestDTO dto)
        {
            var cart = await _cartService.AddItemAsync(GetUserId(), dto.ProductId, dto.Quantity);
            return Ok(cart);
        }

        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> RemoveItem(string itemId)
        {
            var cart = await _cartService.RemoveItemAsync(GetUserId(), itemId);
            return Ok(cart);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var cart = await _cartService.CheckoutAsync(GetUserId());
            return Ok(cart);
        }

        [HttpPut("items/{itemId}")]
        public async Task<IActionResult> UpdateItemQuantity(string itemId, [FromBody] decimal quantity)
        {
            var item = await _cartService.UpdateItemQuantityAsync(GetUserId(), itemId, quantity);
            return Ok(item);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCarts()
        {
            var carts = await _cartService.GetAllCartsAsync(GetUserId());
            return Ok(carts);
        }

    }
}
