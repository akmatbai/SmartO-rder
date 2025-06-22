using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartO_rder.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Article { get; set; } = string.Empty;
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public int StoreId { get; set; }
        public Store? Store { get; set; }
    }
}
