using System.ComponentModel.DataAnnotations;

namespace SmartO_rder.Models
{
    public class StaffForm
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = "Waiter";
        public int CafeId { get; set; }
    }
}
