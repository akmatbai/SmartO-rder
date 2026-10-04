using Microsoft.AspNetCore.Identity;

namespace SmartO_rder.Models
{
    // Links a Cook or Waiter account to the café they work in.
    public class CafeStaff
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public IdentityUser? User { get; set; }
        public int CafeId { get; set; }
        public Cafe? Cafe { get; set; }
    }
}
