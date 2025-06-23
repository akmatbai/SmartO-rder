using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartO_rder.Data;
using SmartO_rder.Models;

namespace SmartO_rder.Controllers
{
    [Route("store/{slug}")]
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StoreController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string slug)
        {
            var store = _context.Stores
                .Include(s => s.Products)
                .FirstOrDefault(s => s.Slug == slug);
            if (store == null)
                return NotFound();
            return View(store);
        }

        [HttpGet("buy/{article}")]
        public IActionResult Buy(string slug, string article)
        {
            var product = _context.Products
                .Include(p => p.Store)
                .FirstOrDefault(p => p.Store!.Slug == slug && p.Article == article);
            if (product == null)
                return NotFound();
            return View("Purchase", product);
        }



// t5ywc2-codex/создать-онлайн-магазин-с-оплатой-по-артикулу-и-qr-меню


        [HttpPost("buy/{article}")]
        public IActionResult Buy(string slug, string article, int quantity)
        {
            var product = _context.Products
                .Include(p => p.Store)
                .FirstOrDefault(p => p.Store!.Slug == slug && p.Article == article);
            if (product == null)
                return NotFound();
            if (quantity <= 0 || quantity > product.Quantity)
            {
                ModelState.AddModelError(string.Empty, "Invalid quantity");
                return View("Purchase", product);
            }

            product.Quantity -= quantity;
            var order = new Order
            {
                ProductId = product.Id,
                Quantity = quantity,
                UserId = User.Identity?.Name ?? "guest"
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            return RedirectToAction("Index", new { slug });
        }


    }
}
