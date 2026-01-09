namespace ShoppingCart.Application.DTOs.Response
{
    public class CartItemResponseDTO
    {
        public decimal Quantity { get; set; }
        public string Id { get; set; }
        public string CartId { get; set; }
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }
}
