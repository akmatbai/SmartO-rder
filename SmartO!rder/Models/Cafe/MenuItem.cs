using System.ComponentModel.DataAnnotations;

namespace SmartO_rder.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;

        public int CafeId { get; set; }
        public Cafe? Cafe { get; set; }
    }
}
