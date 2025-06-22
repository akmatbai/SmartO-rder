using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
ю
using Microsoft.AspNetCore.Identity;


namespace SmartO_rder.Models
{
    public class Cafe
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Slug { get; set; } = string.Empty;


        public string OwnerId { get; set; } = string.Empty;
        public IdentityUser? Owner { get; set; }


        public ICollection<Table> Tables { get; set; } = new List<Table>();
    }
}
