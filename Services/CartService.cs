using Microsoft.EntityFrameworkCore;
using ShoppingCart.Application.DTOs.Response;
using ShoppingCart.EntityCore.Data;
using ShoppingCart.EntityCore.Entities;

namespace ShoppingCart.Services
{
    public class CartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        // get any active/not-checked-out cart and create if there is none
        public async Task<CartResponseDTO> GetOrCreateCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Where(c => c.UserId == userId && !c.IsCheckedOut)
                .Select(c => new CartResponseDTO
                {
                    Id = c.Id,
                    IsCheckedOut = c.IsCheckedOut,
                    CreatedDate = c.CreatedDate,
                    CheckedOutDate = c.CheckedOutDate,
                    Items = c.Items.Select(i => new CartItemResponseDTO
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductCode = i.Product.Code,
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        Price = i.Product.Price
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (cart != null)
                return cart;

            // Create new cart if no active cart
            var newCart = new Cart
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                CreatedDate = DateTime.UtcNow,
                IsCheckedOut = false
            };

            _context.Carts.Add(newCart);
            await _context.SaveChangesAsync();

            // Return new cart with no item
            return new CartResponseDTO
            {
                Id = newCart.Id,
                IsCheckedOut = false,
                CreatedDate = newCart.CreatedDate,
                Items = new List<CartItemResponseDTO>()
            };
        }


        // Add item to cart
        public async Task<CartResponseDTO> AddItemAsync(string userId, string productId, decimal quantity)
        {
            if (quantity <= 0)
                throw new Exception("Quantity must be greater than zero");

            // check active cart
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsCheckedOut);

            // Create cart if there is none
            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow,
                    IsCheckedOut = false
                };
                _context.Carts.Add(cart);
            }

            //Check product exist
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
                throw new Exception("Product not found");

            // Add or update item
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
                existingItem.Quantity += quantity;
            else
                cart.Items.Add(new CartItem
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = productId,
                    Quantity = quantity
                });

            await _context.SaveChangesAsync();

            //return the cart with updated item list
            return new CartResponseDTO
            {
                Id = cart.Id,
                IsCheckedOut = cart.IsCheckedOut,
                CreatedDate = cart.CreatedDate,
                Items = cart.Items.Select(i => new CartItemResponseDTO
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductCode = i.Product.Code,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    Price = i.Product.Price
                }).ToList()
            };
        }

        // Remove item from cart
        public async Task<CartResponseDTO> RemoveItemAsync(string userId, string itemId)
        {
            string cartId = null;
            var item = _context.CartItems.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                cartId = item.CartId;
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            //get the cart with changed item list
            var cart = _context.Carts
                .Where(c => c.Id == cartId)
                .Select(c => new CartResponseDTO
                {
                    Id = c.Id,
                    IsCheckedOut = c.IsCheckedOut,
                    CreatedDate = c.CreatedDate,
                    CheckedOutDate = c.CheckedOutDate,
                    Items = c.Items.Select(i => new CartItemResponseDTO
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductCode = i.Product.Code,
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        Price = i.Product.Price
                    }).ToList()
                })
                .FirstOrDefault();

            return cart;
        }

        // Checkout
        public async Task<CartResponseDTO> CheckoutAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsCheckedOut);

            if (cart == null || !cart.Items.Any())
                throw new Exception("Cart is empty or does not exist");

            // checked out
            cart.IsCheckedOut = true;
            cart.CheckedOutDate = DateTime.UtcNow;

            // reduce product stock balance
            foreach (var item in cart.Items)
            {
                item.Product.Balance -= item.Quantity;
            }

            await _context.SaveChangesAsync();

            //return checked out cart
            var cartDto = new CartResponseDTO
            {
                Id = cart.Id,
                IsCheckedOut = cart.IsCheckedOut,
                CreatedDate = cart.CreatedDate,
                CheckedOutDate = cart.CheckedOutDate,
                Items = cart.Items.Select(i => new CartItemResponseDTO
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductCode = i.Product.Code,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    Price = i.Product.Price
                }).ToList()
            };

            return cartDto;
        }

        public async Task<CartItemResponseDTO> UpdateItemQuantityAsync(string userId, string itemId, decimal quantity)
        {
            var item = await _context.CartItems
                        .Include(ci => ci.Cart)
                        .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.Cart.UserId == userId && !ci.Cart.IsCheckedOut);


            if (item == null)
                throw new Exception("Item not found in active cart");

            if (quantity <= 0)
                _context.CartItems.Remove(item);
            else
                item.Quantity = quantity;

            await _context.SaveChangesAsync();


            var itemDto = new CartItemResponseDTO
            {
                Id = item.Id,
                ProductId = item.ProductId,
                CartId = item.CartId,
                Quantity = item.Quantity
            };

            return itemDto;
        }

        public async Task<List<CartResponseDTO>> GetAllCartsAsync(string userId)
        {
            return await _context.Carts
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedDate)
                .Select(c => new CartResponseDTO
                {
                    Id = c.Id,
                    IsCheckedOut = c.IsCheckedOut,
                    CreatedDate = c.CreatedDate,
                    CheckedOutDate = c.CheckedOutDate,
                    Items = c.Items.Select(i => new CartItemResponseDTO
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductCode = i.Product.Code,
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        Price = i.Product.Price
                    }).ToList()
                })
                .ToListAsync();
        }




    }
}
