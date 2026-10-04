using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartO_rder.Data;
using SmartO_rder.Models;
using System.Linq;

namespace SmartO_rder.Controllers
{
    [Authorize(Roles = "Cook")]
    [Route("cook")]
    public class CookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CookController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("orders")]
        public IActionResult Orders()
        {
            var cafeIds = _context.CafeIdsForStaff(_userManager.GetUserId(User)!);
            var orders = _context.CafeOrders
                .Include(o => o.Items)
                .Include(o => o.Cafe)
                .Where(o => cafeIds.Contains(o.CafeId) && o.Status == CafeOrderStatus.New)
                .OrderBy(o => o.CreatedAt)
                .ToList();
            return View(orders);
        }

        [HttpPost("ready/{id}")]
        public IActionResult MarkReady(int id)
        {
            var cafeIds = _context.CafeIdsForStaff(_userManager.GetUserId(User)!);
            var order = _context.CafeOrders.FirstOrDefault(o => o.Id == id && cafeIds.Contains(o.CafeId));
            if (order == null)
                return NotFound();
            if (order.Status == CafeOrderStatus.New)
            {
                order.Status = CafeOrderStatus.Ready;
                order.ReadyAt = DateTime.UtcNow;
                _context.SaveChanges();
            }
            return RedirectToAction("Orders");
        }
    }
}
