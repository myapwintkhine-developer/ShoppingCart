using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.EntityCore.Entities
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal Balance { get; set; }
    }
}
