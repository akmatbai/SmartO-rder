using Microsoft.AspNetCore.Authorization;
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

        public WaiterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("calls")]
        public IActionResult Calls()
        {
            var calls = _context.Tables.Include(t => t.Cafe).Where(t => t.WaiterCalled).ToList();
            return View(calls);
        }

        [HttpPost("confirm/{id}")]
        public IActionResult ConfirmCall(int id)
        {
            var table = _context.Tables.Find(id);
            if (table != null)
            {
                table.WaiterCalled = false;
                _context.SaveChanges();
            }
            return RedirectToAction("Calls");
        }

        [HttpGet("orders")]
        public IActionResult Orders()
        {
            var orders = _context.Orders.Include(o => o.Product).Where(o => o.IsReady && !o.IsServed).ToList();
            return View(orders);
        }

        [HttpPost("serve/{id}")]
        public IActionResult MarkServed(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                order.IsServed = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Orders");
        }
    }
}
