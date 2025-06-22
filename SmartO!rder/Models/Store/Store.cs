using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartO_rder.Models
{
    public class Store
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Slug { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
