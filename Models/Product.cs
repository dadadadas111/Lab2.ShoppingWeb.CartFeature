using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Lab2.ShoppingWeb.CartFeature.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // New fields
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

        public string QuantityPerUnit { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } = 0;

        public int UnitsOnOrder { get; set; } = 0;

        public int ReorderLevel { get; set; } = 0;

        public bool Discontinued { get; set; } = false;
    }
}
