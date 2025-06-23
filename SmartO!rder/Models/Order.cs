using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SmartO_rder.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        public string UserId { get; set; } = string.Empty;
        public IdentityUser? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsReady { get; set; }
        public bool IsServed { get; set; }
    }
}
