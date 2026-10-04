using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartO_rder.Data;
using SmartO_rder.Models;

namespace SmartO_rder.Controllers
{
    [Authorize(Roles = "CafeMerchant,StoreMerchant")]
    [Route("merchant")]
    public class MerchantController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MerchantController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string UserId => _userManager.GetUserId(User)!;

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            var userId = UserId;
            ViewBag.Stores = _context.Stores.Where(s => s.OwnerId == userId).OrderBy(s => s.Name).ToList();
            ViewBag.Cafes = _context.Cafes.Where(c => c.OwnerId == userId).OrderBy(c => c.Name).ToList();
            ViewBag.CafeOrders = _context.CafeOrders
                .Include(o => o.Items)
                .Include(o => o.Cafe)
                .Where(o => o.Cafe!.OwnerId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Take(50)
                .ToList();
            var orders = _context.Orders
                .Include(o => o.Product)!
                .ThenInclude(p => p!.Store)
                .Where(o => o.Product!.Store!.OwnerId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Take(100)
                .ToList();
            return View(orders);
        }

        [HttpPost("orders/{id}/complete")]
        public IActionResult CompleteOrder(int id)
        {
            var order = FindOwnOrder(id);
            if (order == null)
                return NotFound();
            if (order.Status == StoreOrderStatus.Paid)
            {
                order.Status = StoreOrderStatus.Completed;
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }

        [HttpPost("orders/{id}/cancel")]
        public IActionResult CancelOrder(int id)
        {
            var order = FindOwnOrder(id);
            if (order == null)
                return NotFound();
            // Paid orders would need a refund through the payment provider, so only unpaid ones can be cancelled here.
            using var transaction = _context.Database.BeginTransaction();
            // Conditional update: of two simultaneous cancels only one changes the status and returns the stock.
            var cancelled = _context.Orders
                .Where(o => o.Id == order.Id && o.Status == StoreOrderStatus.AwaitingPayment)
                .ExecuteUpdate(s => s.SetProperty(o => o.Status, StoreOrderStatus.Cancelled));
            if (cancelled == 1)
            {
                _context.Products.Where(p => p.Id == order.ProductId)
                    .ExecuteUpdate(s => s.SetProperty(p => p.Quantity, p => p.Quantity + order.Quantity));
            }
            transaction.Commit();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("products")]
        public IActionResult Products()
        {
            var userId = UserId;
            var products = _context.Products
                .Include(p => p.Store)
                .Where(p => p.Store!.OwnerId == userId)
                .OrderBy(p => p.Store!.Name).ThenBy(p => p.Name)
                .ToList();
            return View(products);
        }

        [HttpGet("add-product")]
        public IActionResult AddProduct()
        {
            ViewBag.Stores = OwnStores();
            return View();
        }

        [HttpPost("add-product")]
        public IActionResult AddProduct(Product product)
        {
            ValidateProduct(product, excludeId: null);
            if (ModelState.IsValid)
            {
                product.Id = 0;
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Products");
            }
            ViewBag.Stores = OwnStores();
            return View(product);
        }

        [HttpGet("products/{id}/edit")]
        public IActionResult EditProduct(int id)
        {
            var product = FindOwnProduct(id);
            if (product == null)
                return NotFound();
            ViewBag.Stores = OwnStores();
            return View(product);
        }

        [HttpPost("products/{id}/edit")]
        public IActionResult EditProduct(int id, Product input)
        {
            var product = FindOwnProduct(id);
            if (product == null)
                return NotFound();

            ValidateProduct(input, excludeId: id);
            if (!ModelState.IsValid)
            {
                input.Id = id;
                ViewBag.Stores = OwnStores();
                return View(input);
            }

            product.Name = input.Name;
            product.Article = input.Article;
            product.Price = input.Price;
            product.Quantity = input.Quantity;
            product.Category = input.Category;
            product.ImageUrl = input.ImageUrl;
            product.StoreId = input.StoreId;
            _context.SaveChanges();
            return RedirectToAction("Products");
        }

        [HttpPost("products/{id}/delete")]
        public IActionResult DeleteProduct(int id)
        {
            var product = FindOwnProduct(id);
            if (product == null)
                return NotFound();
            if (_context.Orders.Any(o => o.ProductId == id))
            {
                TempData["Error"] = $"\"{product.Name}\" has orders and cannot be deleted. Set its quantity to 0 to stop selling it.";
                return RedirectToAction("Products");
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Products");
        }

        private List<Store> OwnStores()
        {
            var userId = UserId;
            return _context.Stores.Where(s => s.OwnerId == userId).ToList();
        }

        private Product? FindOwnProduct(int id)
        {
            var userId = UserId;
            return _context.Products.FirstOrDefault(p => p.Id == id && p.Store!.OwnerId == userId);
        }

        private Order? FindOwnOrder(int id)
        {
            var userId = UserId;
            return _context.Orders
                .FirstOrDefault(o => o.Id == id && o.Product!.Store!.OwnerId == userId);
        }

        private void ValidateProduct(Product product, int? excludeId)
        {
            var userId = UserId;
            if (!_context.Stores.Any(s => s.Id == product.StoreId && s.OwnerId == userId))
                ModelState.AddModelError(nameof(Product.StoreId), "Store not found");
            else if (_context.Products.Any(p => p.StoreId == product.StoreId && p.Article == product.Article && p.Id != excludeId))
                ModelState.AddModelError(nameof(Product.Article), "Article already exists in this store");
        }
    }
}
