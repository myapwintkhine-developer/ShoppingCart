namespace ShoppingCart.Application.DTOs.Response
{
    public class ProductResponseDTO
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public decimal Balance { get; set; }
    }
}
