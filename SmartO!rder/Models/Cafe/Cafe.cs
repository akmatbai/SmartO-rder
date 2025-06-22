using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartO_rder.Models
{
    public class Cafe
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Slug { get; set; } = string.Empty;

        public ICollection<Table> Tables { get; set; } = new List<Table>();
    }
}
