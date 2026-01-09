using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.EntityCore.Entities
{
    [Table("CartItem")]
    public class CartItem
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string CartId { get; set; }

        [ForeignKey("CartId")]
        public Cart Cart { get; set; }

        [Required]
        public string ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        public decimal Quantity { get; set; }

    }
}
