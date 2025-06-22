using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartO_rder.Data;
using SmartO_rder.Models;

namespace SmartO_rder.Controllers
{

    [Authorize(Roles = "CafeMerchant,StoreMerchant")]

    [Authorize(Roles = "Merchant")]

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

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            var userId = _userManager.GetUserId(User)!;
            var orders = _context.Orders
                .Include(o => o.Product)!
                .ThenInclude(p => p!.Store)
                .Where(o => o.Product!.Store!.OwnerId == userId)
                .ToList();
            return View(orders);
        }

        [HttpGet("add-product")]
        public IActionResult AddProduct()
        {
            var userId = _userManager.GetUserId(User)!;
            ViewBag.Stores = _context.Stores.Where(s => s.OwnerId == userId).ToList();
            return View();
        }

        [HttpPost("add-product")]
        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            var userId = _userManager.GetUserId(User)!;
            ViewBag.Stores = _context.Stores.Where(s => s.OwnerId == userId).ToList();
            return View(product);
        }

        [HttpGet("add-table")]
        public IActionResult AddTable()
        {
            var userId = _userManager.GetUserId(User)!;
            ViewBag.Cafes = _context.Cafes.Where(c => c.OwnerId == userId).ToList();
            return View();
        }

        [HttpPost("add-table")]
        public IActionResult AddTable(Table table)
        {
            if (ModelState.IsValid)
            {
                _context.Tables.Add(table);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            var userId = _userManager.GetUserId(User)!;
            ViewBag.Cafes = _context.Cafes.Where(c => c.OwnerId == userId).ToList();
            return View(table);
        }
    }
}
