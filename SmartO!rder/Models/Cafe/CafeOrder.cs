using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SmartO_rder.Models
{
    public enum CafeOrderStatus
    {
        New,
        Ready,
        Served
    }

    public class CafeOrder
    {
        public int Id { get; set; }
        // Shown to the guest in the status page URL; not guessable like Id.
        public Guid PublicId { get; set; } = Guid.NewGuid();

        public int CafeId { get; set; }
        public Cafe? Cafe { get; set; }

        // The table may be deleted later; TableNumber keeps the history readable.
        public int? TableId { get; set; }
        public Table? Table { get; set; }
        public int TableNumber { get; set; }

        public string? Comment { get; set; }
        public CafeOrderStatus Status { get; set; } = CafeOrderStatus.New;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadyAt { get; set; }
        public DateTime? ServedAt { get; set; }

        public ICollection<CafeOrderItem> Items { get; set; } = new List<CafeOrderItem>();

        [NotMapped]
        public decimal Total => Items.Sum(i => i.UnitPrice * i.Quantity);
    }

    public class CafeOrderItem
    {
        public int Id { get; set; }
        public int CafeOrderId { get; set; }
        public CafeOrder? CafeOrder { get; set; }

        // Name and price are copied so that editing or deleting the menu item keeps the order intact.
        public int? MenuItemId { get; set; }
        public MenuItem? MenuItem { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
