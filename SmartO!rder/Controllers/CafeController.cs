using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartO_rder.Data;
using SmartO_rder.Models;

namespace SmartO_rder.Controllers
{
    [Route("cafe/{slug}")]
    public class CafeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CafeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("menu/{table}")]
        public IActionResult Menu(string slug, int table)
        {
            var cafe = _context.Cafes
                .Include(c => c.Tables)
                .FirstOrDefault(c => c.Slug == slug);
            if (cafe == null)
                return NotFound();
            var tableEntity = cafe.Tables.FirstOrDefault(t => t.Number == table);
            if (tableEntity == null)
                return NotFound();

            ViewBag.Table = tableEntity;
            var products = _context.Products.ToList();
            return View(products);
        }

        [HttpPost("call/{table}")]
        public IActionResult CallWaiter(string slug, int table)
        {
            var tableEntity = _context.Tables
                .Include(t => t.Cafe)
                .FirstOrDefault(t => t.Cafe!.Slug == slug && t.Number == table);
            if (tableEntity == null)
                return NotFound();

            tableEntity.WaiterCalled = true;
            _context.SaveChanges();

            return RedirectToAction("Menu", new { slug, table });
        }
    }
}
