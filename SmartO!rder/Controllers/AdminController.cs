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
        private readonly ApplicationDbContext _context;




        public async Task<IActionResult> Dashboard()
            var storeMerchants = await _userManager.GetUsersInRoleAsync("StoreMerchant");
            var cafeMerchants = await _userManager.GetUsersInRoleAsync("CafeMerchant");
            var allMerchants = storeMerchants.Concat(cafeMerchants).ToList();
            ViewBag.Stores = await _context.Stores.Include(s => s.Owner).ToListAsync();
            ViewBag.Cafes = await _context.Cafes.Include(c => c.Owner).ToListAsync();
            return View(allMerchants);

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Route("dashboard")]

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


        public IActionResult Dashboard()
        {

            var storeMerchants = _userManager.GetUsersInRoleAsync("StoreMerchant").Result;
            var cafeMerchants = _userManager.GetUsersInRoleAsync("CafeMerchant").Result;
            var merchants = storeMerchants.Concat(cafeMerchants).ToList();

            var merchants = _userManager.GetUsersInRoleAsync("Merchant").Result;

            ViewBag.Stores = _context.Stores.Include(s => s.Owner).ToList();
            ViewBag.Cafes = _context.Cafes.Include(c => c.Owner).ToList();
            return View(merchants);
        }

        [HttpGet("add-merchant")]

        public IActionResult AddMerchant()
        {
            ViewBag.Roles = new[] { "CafeMerchant", "StoreMerchant" };
            return View();
        }

        [HttpPost("add-merchant")]
        public async Task<IActionResult> AddMerchant(string email, string password, string role)



        public IActionResult AddMerchant() => View();

        [HttpPost("add-merchant")]
        public async Task<IActionResult> AddMerchant(string email, string password)


        {
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));
                await _userManager.AddToRoleAsync(user, role);


                if (!await _roleManager.RoleExistsAsync("Merchant"))
                    await _roleManager.CreateAsync(new IdentityRole("Merchant"));
                await _userManager.AddToRoleAsync(user, "Merchant");

                return RedirectToAction("Dashboard");
            }
            foreach (var e in result.Errors)
                ModelState.AddModelError(string.Empty, e.Description);
            ViewBag.Roles = new[] { "CafeMerchant", "StoreMerchant" };
            return View();
        }

        [HttpGet("create-store")]
        public IActionResult CreateStore()
        {
            ViewBag.Merchants = _userManager.GetUsersInRoleAsync("StoreMerchant").Result;


            ViewBag.Merchants = _userManager.GetUsersInRoleAsync("Merchant").Result;


            return View();
        }

        [HttpPost("create-store")]
        public IActionResult CreateStore(Store store)
        {
            if (ModelState.IsValid)
            {
                _context.Stores.Add(store);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            ViewBag.Merchants = _userManager.GetUsersInRoleAsync("StoreMerchant").Result;


            ViewBag.Merchants = _userManager.GetUsersInRoleAsync("Merchant").Result;


            return View(store);
        }

        [HttpGet("create-cafe")]
        public IActionResult CreateCafe()
        {
            ViewBag.Merchants = _userManager.GetUsersInRoleAsync("CafeMerchant").Result;


            ViewBag.Merchants = _userManager.GetUsersInRoleAsync("Merchant").Result;

            return View();
        }

        [HttpPost("create-cafe")]
        public IActionResult CreateCafe(Cafe cafe)
        {
            if (ModelState.IsValid)
            {
                _context.Cafes.Add(cafe);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            ViewBag.Merchants = _userManager.GetUsersInRoleAsync("CafeMerchant").Result;


            ViewBag.Merchants = _userManager.GetUsersInRoleAsync("Merchant").Result;

            return View(cafe);
        }
    }
}
