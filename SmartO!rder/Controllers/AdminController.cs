using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartO_rder.Data;
using SmartO_rder.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SmartO_rder.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Route("admin")]
    public class AdminController : Controller
    {
        private static readonly string[] MerchantRoles = { "CafeMerchant", "StoreMerchant" };

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var storeMerchants = await _userManager.GetUsersInRoleAsync("StoreMerchant");
            var cafeMerchants = await _userManager.GetUsersInRoleAsync("CafeMerchant");
            var allMerchants = storeMerchants.Concat(cafeMerchants).ToList();
            ViewBag.Stores = await _context.Stores.Include(s => s.Owner).ToListAsync();
            ViewBag.Cafes = await _context.Cafes.Include(c => c.Owner).ToListAsync();
            return View(allMerchants);
        }

        [HttpGet("add-merchant")]
        public IActionResult AddMerchant()
        {
            ViewBag.Roles = MerchantRoles;
            return View();
        }

        [HttpPost("add-merchant")]
        public async Task<IActionResult> AddMerchant(string email, string password, string role)
        {
            if (!MerchantRoles.Contains(role))
                ModelState.AddModelError(string.Empty, "Unknown role");

            if (ModelState.IsValid)
            {
                // Created by an administrator, so there is no confirmation e-mail to wait for.
                var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    await _userManager.AddToRoleAsync(user, role);
                    return RedirectToAction("Dashboard");
                }
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);
            }
            ViewBag.Roles = MerchantRoles;
            return View();
        }

        [HttpGet("create-store")]
        public async Task<IActionResult> CreateStore()
        {
            ViewBag.Merchants = await _userManager.GetUsersInRoleAsync("StoreMerchant");
            return View();
        }

        [HttpPost("create-store")]
        public async Task<IActionResult> CreateStore(Store store)
        {
            if (await _context.Stores.AnyAsync(s => s.Slug == store.Slug))
                ModelState.AddModelError(nameof(Store.Slug), "Slug is already taken");

            if (ModelState.IsValid)
            {
                _context.Stores.Add(store);
                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard");
            }
            ViewBag.Merchants = await _userManager.GetUsersInRoleAsync("StoreMerchant");
            return View(store);
        }

        [HttpGet("create-cafe")]
        public async Task<IActionResult> CreateCafe()
        {
            ViewBag.Merchants = await _userManager.GetUsersInRoleAsync("CafeMerchant");
            return View();
        }

        [HttpPost("create-cafe")]
        public async Task<IActionResult> CreateCafe(Cafe cafe)
        {
            if (await _context.Cafes.AnyAsync(c => c.Slug == cafe.Slug))
                ModelState.AddModelError(nameof(Cafe.Slug), "Slug is already taken");

            if (ModelState.IsValid)
            {
                _context.Cafes.Add(cafe);
                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard");
            }
            ViewBag.Merchants = await _userManager.GetUsersInRoleAsync("CafeMerchant");
            return View(cafe);
        }
    }
}
