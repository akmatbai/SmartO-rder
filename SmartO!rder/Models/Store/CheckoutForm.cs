using System.ComponentModel.DataAnnotations;

namespace SmartO_rder.Models
{
    public class CheckoutForm
    {
        [Range(1, 1000)]
        public int Quantity { get; set; } = 1;
        [Required, StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        [Required, Phone, StringLength(30)]
        public string Phone { get; set; } = string.Empty;
        public DeliveryMethod DeliveryMethod { get; set; } = DeliveryMethod.Pickup;
        [StringLength(300)]
        public string? Address { get; set; }
    }

    public class PurchaseViewModel
    {
        public Product Product { get; set; } = default!;
        public CheckoutForm Form { get; set; } = new();
    }
}
