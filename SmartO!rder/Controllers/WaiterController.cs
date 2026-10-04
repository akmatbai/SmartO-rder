using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartO_rder.Data;
using SmartO_rder.Models;
using System.Linq;

namespace SmartO_rder.Controllers
{
    [Authorize(Roles = "Waiter")]
    [Route("waiter")]
    public class WaiterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public WaiterController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private IQueryable<int> MyCafeIds() => _context.CafeIdsForStaff(_userManager.GetUserId(User)!);

        [HttpGet("calls")]
        public IActionResult Calls()
        {
            var cafeIds = MyCafeIds();
            var calls = _context.Tables
                .Include(t => t.Cafe)
                .Where(t => t.WaiterCalled && cafeIds.Contains(t.CafeId))
                .OrderBy(t => t.WaiterCalledAt)
                .ToList();
            return View(calls);
        }

        [HttpPost("confirm/{id}")]
        public IActionResult ConfirmCall(int id)
        {
            var cafeIds = MyCafeIds();
            var table = _context.Tables.FirstOrDefault(t => t.Id == id && cafeIds.Contains(t.CafeId));
            if (table == null)
                return NotFound();
            table.WaiterCalled = false;
            table.WaiterCalledAt = null;
            _context.SaveChanges();
            return RedirectToAction("Calls");
        }

        [HttpGet("orders")]
        public IActionResult Orders()
        {
            var cafeIds = MyCafeIds();
            var orders = _context.CafeOrders
                .Include(o => o.Items)
                .Include(o => o.Cafe)
                .Where(o => cafeIds.Contains(o.CafeId) && o.Status == CafeOrderStatus.Ready)
                .OrderBy(o => o.ReadyAt)
                .ToList();
            return View(orders);
        }

        [HttpPost("serve/{id}")]
        public IActionResult MarkServed(int id)
        {
            var cafeIds = MyCafeIds();
            var order = _context.CafeOrders.FirstOrDefault(o => o.Id == id && cafeIds.Contains(o.CafeId));
            if (order == null)
                return NotFound();
            if (order.Status == CafeOrderStatus.Ready)
            {
                order.Status = CafeOrderStatus.Served;
                order.ServedAt = DateTime.UtcNow;
                _context.SaveChanges();
            }
            return RedirectToAction("Orders");
        }
    }
}
