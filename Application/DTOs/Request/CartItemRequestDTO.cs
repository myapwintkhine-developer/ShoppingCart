namespace ShoppingCart.Application.DTOs.Request
{
    public class CartItemRequestDTO
    {
        public string? Id { get; set; }
        public string? CartId { get; set; }
        public string ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
