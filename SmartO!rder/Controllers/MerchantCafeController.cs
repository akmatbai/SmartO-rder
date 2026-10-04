using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using SmartO_rder.Data;
using SmartO_rder.Models;

namespace SmartO_rder.Controllers
{
    [Authorize(Roles = "CafeMerchant")]
    [Route("merchant/cafe")]
    public class MerchantCafeController : Controller
    {
        public static readonly string[] StaffRoles = { "Cook", "Waiter" };

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MerchantCafeController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string UserId => _userManager.GetUserId(User)!;

        private List<Cafe> OwnCafes()
        {
            var userId = UserId;
            return _context.Cafes.Where(c => c.OwnerId == userId).OrderBy(c => c.Name).ToList();
        }

        private bool OwnsCafe(int cafeId)
        {
            var userId = UserId;
            return _context.Cafes.Any(c => c.Id == cafeId && c.OwnerId == userId);
        }

        // ---- Menu ----

        [HttpGet("menu")]
        public IActionResult Menu()
        {
            var userId = UserId;
            var items = _context.MenuItems
                .Include(m => m.Cafe)
                .Where(m => m.Cafe!.OwnerId == userId)
                .OrderBy(m => m.Cafe!.Name).ThenBy(m => m.Category).ThenBy(m => m.Name)
                .ToList();
            return View(items);
        }

        [HttpGet("menu/add")]
        public IActionResult AddMenuItem()
        {
            ViewBag.Cafes = OwnCafes();
            return View("EditMenuItem", new MenuItem());
        }

        [HttpPost("menu/add")]
        public IActionResult AddMenuItem(MenuItem item)
        {
            if (!OwnsCafe(item.CafeId))
                ModelState.AddModelError(nameof(MenuItem.CafeId), "Cafe not found");
            if (!ModelState.IsValid)
            {
                ViewBag.Cafes = OwnCafes();
                return View("EditMenuItem", item);
            }
            item.Id = 0;
            _context.MenuItems.Add(item);
            _context.SaveChanges();
            return RedirectToAction("Menu");
        }

        [HttpGet("menu/{id}/edit")]
        public IActionResult EditMenuItem(int id)
        {
            var item = FindOwnMenuItem(id);
            if (item == null)
                return NotFound();
            ViewBag.Cafes = OwnCafes();
            return View(item);
        }

        [HttpPost("menu/{id}/edit")]
        public IActionResult EditMenuItem(int id, MenuItem input)
        {
            var item = FindOwnMenuItem(id);
            if (item == null)
                return NotFound();
            if (!OwnsCafe(input.CafeId))
                ModelState.AddModelError(nameof(MenuItem.CafeId), "Cafe not found");
            if (!ModelState.IsValid)
            {
                input.Id = id;
                ViewBag.Cafes = OwnCafes();
                return View(input);
            }

            item.Name = input.Name;
            item.Description = input.Description;
            item.Category = input.Category;
            item.Price = input.Price;
            item.ImageUrl = input.ImageUrl;
            item.IsAvailable = input.IsAvailable;
            item.CafeId = input.CafeId;
            _context.SaveChanges();
            return RedirectToAction("Menu");
        }

        [HttpPost("menu/{id}/delete")]
        public IActionResult DeleteMenuItem(int id)
        {
            var item = FindOwnMenuItem(id);
            if (item == null)
                return NotFound();
            // Past orders keep a copy of the name and price, so deleting is safe.
            _context.MenuItems.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("Menu");
        }

        private MenuItem? FindOwnMenuItem(int id)
        {
            var userId = UserId;
            return _context.MenuItems.FirstOrDefault(m => m.Id == id && m.Cafe!.OwnerId == userId);
        }

        // ---- Tables and QR codes ----

        [HttpGet("tables")]
        public IActionResult Tables()
        {
            var userId = UserId;
            var tables = _context.Tables
                .Include(t => t.Cafe)
                .Where(t => t.Cafe!.OwnerId == userId)
                .OrderBy(t => t.Cafe!.Name).ThenBy(t => t.Number)
                .ToList();
            return View(tables);
        }

        [HttpGet("tables/add")]
        public IActionResult AddTable()
        {
            ViewBag.Cafes = OwnCafes();
            return View();
        }

        [HttpPost("tables/add")]
        public IActionResult AddTable(Table table)
        {
            if (!OwnsCafe(table.CafeId))
                ModelState.AddModelError(nameof(Table.CafeId), "Cafe not found");
            else if (_context.Tables.Any(t => t.CafeId == table.CafeId && t.Number == table.Number))
                ModelState.AddModelError(nameof(Table.Number), "Table number already exists");
            if (table.Number <= 0)
                ModelState.AddModelError(nameof(Table.Number), "Table number must be positive");

            if (ModelState.IsValid)
            {
                table.Id = 0;
                table.WaiterCalled = false;
                table.WaiterCalledAt = null;
                _context.Tables.Add(table);
                _context.SaveChanges();
                return RedirectToAction("Tables");
            }
            ViewBag.Cafes = OwnCafes();
            return View(table);
        }

        [HttpPost("tables/{id}/delete")]
        public IActionResult DeleteTable(int id)
        {
            var table = FindOwnTable(id);
            if (table == null)
                return NotFound();
            // Orders keep the table number; their TableId becomes null.
            _context.Tables.Remove(table);
            _context.SaveChanges();
            return RedirectToAction("Tables");
        }

        [HttpGet("tables/{id}/qr.png")]
        public IActionResult TableQr(int id)
        {
            var table = FindOwnTable(id);
            if (table == null)
                return NotFound();
            var url = MenuUrl(table);
            var png = PngByteQRCodeHelper.GetQRCode(url, QRCodeGenerator.ECCLevel.Q, 10);
            return File(png, "image/png", $"{table.Cafe!.Slug}-table-{table.Number}.png");
        }

        [HttpGet("tables/print")]
        public IActionResult PrintQr(int cafeId)
        {
            if (!OwnsCafe(cafeId))
                return NotFound();
            var tables = _context.Tables.Include(t => t.Cafe)
                .Where(t => t.CafeId == cafeId)
                .OrderBy(t => t.Number)
                .ToList();
            ViewBag.Urls = tables.ToDictionary(t => t.Id, MenuUrl);
            return View(tables);
        }

        private Table? FindOwnTable(int id)
        {
            var userId = UserId;
            return _context.Tables.Include(t => t.Cafe)
                .FirstOrDefault(t => t.Id == id && t.Cafe!.OwnerId == userId);
        }

        private string MenuUrl(Table table) =>
            Url.Action("Menu", "Cafe", new { slug = table.Cafe!.Slug, table = table.Number }, Request.Scheme)!;

        // ---- Staff ----

        [HttpGet("staff")]
        public async Task<IActionResult> Staff()
        {
            var userId = UserId;
            var staff = _context.CafeStaff
                .Include(s => s.User)
                .Include(s => s.Cafe)
                .Where(s => s.Cafe!.OwnerId == userId)
                .OrderBy(s => s.Cafe!.Name).ThenBy(s => s.User!.Email)
                .ToList();
            var roles = new Dictionary<string, string>();
            foreach (var s in staff)
                roles[s.UserId] = string.Join(", ", (await _userManager.GetRolesAsync(s.User!)).Intersect(StaffRoles));
            ViewBag.Roles = roles;
            return View(staff);
        }

        [HttpGet("staff/add")]
        public IActionResult AddStaff()
        {
            ViewBag.Cafes = OwnCafes();
            return View(new StaffForm());
        }

        [HttpPost("staff/add")]
        public async Task<IActionResult> AddStaff(StaffForm form)
        {
            if (!StaffRoles.Contains(form.Role))
                ModelState.AddModelError(nameof(StaffForm.Role), "Unknown role");
            if (!OwnsCafe(form.CafeId))
                ModelState.AddModelError(nameof(StaffForm.CafeId), "Cafe not found");
            if (ModelState.IsValid && await _userManager.FindByEmailAsync(form.Email) != null)
                ModelState.AddModelError(nameof(StaffForm.Email), "A user with this e-mail already exists");

            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = form.Email, Email = form.Email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, form.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, form.Role);
                    _context.CafeStaff.Add(new CafeStaff { UserId = user.Id, CafeId = form.CafeId });
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Staff");
                }
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);
            }
            ViewBag.Cafes = OwnCafes();
            return View(form);
        }

        [HttpPost("staff/{id}/remove")]
        public async Task<IActionResult> RemoveStaff(int id)
        {
            var userId = UserId;
            var link = _context.CafeStaff.Include(s => s.User)
                .FirstOrDefault(s => s.Id == id && s.Cafe!.OwnerId == userId);
            if (link == null)
                return NotFound();

            _context.CafeStaff.Remove(link);
            await _context.SaveChangesAsync();

            // The account was created for this café: delete it once it works nowhere else,
            // unless it also has a non-staff role.
            var user = link.User!;
            var roles = await _userManager.GetRolesAsync(user);
            if (!_context.CafeStaff.Any(s => s.UserId == user.Id) && roles.All(r => StaffRoles.Contains(r)))
                await _userManager.DeleteAsync(user);

            return RedirectToAction("Staff");
        }
    }
}
