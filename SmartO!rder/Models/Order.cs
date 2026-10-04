using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartO_rder.Models
{
    public enum StoreOrderStatus
    {
        AwaitingPayment,
        Paid,
        Completed,
        Cancelled
    }

    public enum DeliveryMethod
    {
        Pickup,
        Delivery
    }

    // An order placed in a store.
    public class Order
    {
        public int Id { get; set; }
        // Used in customer-facing URLs instead of the sequential Id.
        public Guid PublicId { get; set; } = Guid.NewGuid();
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        // null for guest purchases
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DeliveryMethod DeliveryMethod { get; set; }
        public string? Address { get; set; }

        public StoreOrderStatus Status { get; set; } = StoreOrderStatus.AwaitingPayment;
        public string? PaymentReference { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }

        [NotMapped]
        public decimal Total => UnitPrice * Quantity;
    }
}
