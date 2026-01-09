namespace ShoppingCart.Application.DTOs.Response
{
    public class CartResponseDTO
    {
        public string Id { get; set; }
        public bool IsCheckedOut { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? CheckedOutDate { get; set; }

        public ICollection<CartItemResponseDTO> Items { get; set; }
    }
}
