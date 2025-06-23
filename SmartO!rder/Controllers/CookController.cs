using Microsoft.AspNetCore.Authorization;
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

        public CookController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("orders")]
        public IActionResult Orders()
        {
            var orders = _context.Orders.Include(o => o.Product).Where(o => !o.IsReady).ToList();
            return View(orders);
        }

        [HttpPost("ready/{id}")]
        public IActionResult MarkReady(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                order.IsReady = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Orders");
        }
    }
}
