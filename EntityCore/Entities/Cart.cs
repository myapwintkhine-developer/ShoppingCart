using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.EntityCore.Entities
{
    [Table("Cart")]
    public class Cart
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public bool IsCheckedOut { get; set; } = false;

        public DateTime CreatedDate { get; set; }
        public DateTime? CheckedOutDate { get; set; }

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();



    }
}
