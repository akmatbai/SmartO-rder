using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartO_rder.Data;
using SmartO_rder.Models;

namespace SmartO_rder.Controllers
{
    [Route("cafe/{slug}")]
    public class CafeController : Controller
    {
        private const int MaxItemQuantity = 50;

        private readonly ApplicationDbContext _context;

        public CafeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("menu/{table}")]
        public IActionResult Menu(string slug, int table)
        {
            var tableEntity = FindTable(slug, table);
            if (tableEntity == null)
                return NotFound();

            ViewBag.Table = tableEntity;
            var items = _context.MenuItems
                .Where(m => m.CafeId == tableEntity.CafeId && m.IsAvailable)
                .OrderBy(m => m.Category).ThenBy(m => m.Name)
                .ToList();
            return View(items);
        }

        [HttpPost("order/{table}")]
        public IActionResult PlaceOrder(string slug, int table, string? comment)
        {
            var tableEntity = FindTable(slug, table);
            if (tableEntity == null)
                return NotFound();

            var requested = ParseItems(Request.Form);
            var ids = requested.Keys.ToList();
            var menuItems = _context.MenuItems
                .Where(m => m.CafeId == tableEntity.CafeId && m.IsAvailable && ids.Contains(m.Id))
                .ToList();
            if (menuItems.Count == 0)
            {
                TempData["MenuError"] = "Choose at least one dish";
                return RedirectToAction("Menu", new { slug, table });
            }

            var order = new CafeOrder
            {
                CafeId = tableEntity.CafeId,
                TableId = tableEntity.Id,
                TableNumber = tableEntity.Number,
                Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim()[..Math.Min(comment.Trim().Length, 300)],
                Items = menuItems.Select(m => new CafeOrderItem
                {
                    MenuItemId = m.Id,
                    Name = m.Name,
                    UnitPrice = m.Price,
                    Quantity = requested[m.Id]
                }).ToList()
            };
            _context.CafeOrders.Add(order);
            _context.SaveChanges();

            return RedirectToAction("OrderStatus", new { slug, id = order.PublicId });
        }

        [HttpGet("order/{id:guid}")]
        public IActionResult OrderStatus(string slug, Guid id)
        {
            var order = _context.CafeOrders
                .Include(o => o.Items)
                .Include(o => o.Cafe)
                .FirstOrDefault(o => o.PublicId == id && o.Cafe!.Slug == slug);
            if (order == null)
                return NotFound();
            return View(order);
        }

        [HttpPost("call/{table}")]
        public IActionResult CallWaiter(string slug, int table)
        {
            var tableEntity = FindTable(slug, table);
            if (tableEntity == null)
                return NotFound();

            if (!tableEntity.WaiterCalled)
            {
                tableEntity.WaiterCalled = true;
                tableEntity.WaiterCalledAt = DateTime.UtcNow;
                _context.SaveChanges();
            }
            TempData["MenuMessage"] = "The waiter has been called";

            return RedirectToAction("Menu", new { slug, table });
        }

        // Reads "items[<menuItemId>]=<quantity>" fields. Parsed by hand because dictionary model binding
        // falls back to binding every form field when no such field is present, which throws.
        private static Dictionary<int, int> ParseItems(IFormCollection form)
        {
            var result = new Dictionary<int, int>();
            foreach (var (key, value) in form)
            {
                if (!key.StartsWith("items[") || !key.EndsWith("]"))
                    continue;
                if (int.TryParse(key[6..^1], out var id) && int.TryParse(value, out var quantity) && quantity > 0)
                    result[id] = Math.Min(quantity, MaxItemQuantity);
            }
            return result;
        }

        private Table? FindTable(string slug, int number) =>
            _context.Tables
                .Include(t => t.Cafe)
                .FirstOrDefault(t => t.Cafe!.Slug == slug && t.Number == number);
    }
}
