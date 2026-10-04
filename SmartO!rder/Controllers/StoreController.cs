using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartO_rder.Data;
using SmartO_rder.Models;
using SmartO_rder.Services;

namespace SmartO_rder.Controllers
{
    [Route("store/{slug}")]
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IPaymentService _payments;

        public StoreController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IPaymentService payments)
        {
            _context = context;
            _userManager = userManager;
            _payments = payments;
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

        [HttpGet("find")]
        public IActionResult Find(string slug, string? article)
        {
            article = article?.Trim();
            if (string.IsNullOrEmpty(article))
                return RedirectToAction("Index", new { slug });

            var exists = _context.Products
                .Any(p => p.Store!.Slug == slug && p.Article == article);
            if (!exists)
            {
                TempData["SearchError"] = $"Product with article \"{article}\" not found";
                return RedirectToAction("Index", new { slug });
            }
            return RedirectToAction("Buy", new { slug, article });
        }

        [HttpGet("buy/{article}")]
        public IActionResult Buy(string slug, string article)
        {
            var product = FindProduct(slug, article);
            if (product == null)
                return NotFound();
            return View("Purchase", new PurchaseViewModel { Product = product });
        }

        [HttpPost("buy/{article}")]
        public async Task<IActionResult> Buy(string slug, string article, [Bind(Prefix = "Form")] CheckoutForm form)
        {
            var product = FindProduct(slug, article);
            if (product == null)
                return NotFound();

            if (form.DeliveryMethod == DeliveryMethod.Delivery && string.IsNullOrWhiteSpace(form.Address))
                ModelState.AddModelError("Form.Address", "Address is required for delivery");
            if (form.Quantity > product.Quantity)
                ModelState.AddModelError("Form.Quantity", $"Only {product.Quantity} left in stock");
            if (!ModelState.IsValid)
                return View("Purchase", new PurchaseViewModel { Product = product, Form = form });

            await using var transaction = await _context.Database.BeginTransactionAsync();

            // Reserve stock atomically: the update only succeeds if enough items are still left,
            // so two simultaneous buyers can never take the last item twice.
            var reserved = await _context.Products
                .Where(p => p.Id == product.Id && p.Quantity >= form.Quantity)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.Quantity, p => p.Quantity - form.Quantity));
            if (reserved == 0)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("Form.Quantity", "Not enough items in stock");
                _context.Entry(product).Reload();
                return View("Purchase", new PurchaseViewModel { Product = product, Form = form });
            }

            var order = new Order
            {
                ProductId = product.Id,
                Quantity = form.Quantity,
                UnitPrice = product.Price,
                UserId = _userManager.GetUserId(User),
                CustomerName = form.CustomerName.Trim(),
                Phone = form.Phone.Trim(),
                DeliveryMethod = form.DeliveryMethod,
                Address = form.DeliveryMethod == DeliveryMethod.Delivery ? form.Address!.Trim() : null
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return RedirectToAction("Pay", new { slug, id = order.PublicId });
        }

        [HttpGet("pay/{id:guid}")]
        public IActionResult Pay(string slug, Guid id)
        {
            var order = FindOrder(slug, id);
            if (order == null)
                return NotFound();
            if (order.Status != StoreOrderStatus.AwaitingPayment)
                return RedirectToAction("OrderDetails", new { slug, id });
            ViewBag.TestMode = _payments.IsTestMode;
            return View(order);
        }

        [HttpPost("pay/{id:guid}")]
        public async Task<IActionResult> PayPost(string slug, Guid id)
        {
            var order = FindOrder(slug, id);
            if (order == null)
                return NotFound();
            if (order.Status != StoreOrderStatus.AwaitingPayment)
                return RedirectToAction("OrderDetails", new { slug, id });

            var result = await _payments.ChargeAsync(order.Total,
                $"{order.Product!.Store!.Name}: {order.Product.Name} x {order.Quantity}", order.PublicId.ToString());
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Payment failed");
                ViewBag.TestMode = _payments.IsTestMode;
                return View("Pay", order);
            }

            // Only an order still awaiting payment becomes paid (e.g. not one cancelled meanwhile).
            // With a real provider, a charge for an order cancelled in between must be refunded.
            await _context.Orders
                .Where(o => o.Id == order.Id && o.Status == StoreOrderStatus.AwaitingPayment)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(o => o.Status, StoreOrderStatus.Paid)
                    .SetProperty(o => o.PaidAt, DateTime.UtcNow)
                    .SetProperty(o => o.PaymentReference, result.Reference));
            return RedirectToAction("OrderDetails", new { slug, id });
        }

        [HttpGet("order/{id:guid}")]
        public IActionResult OrderDetails(string slug, Guid id)
        {
            var order = FindOrder(slug, id);
            if (order == null)
                return NotFound();
            return View(order);
        }

        private Product? FindProduct(string slug, string article) =>
            _context.Products
                .Include(p => p.Store)
                .FirstOrDefault(p => p.Store!.Slug == slug && p.Article == article);

        private Order? FindOrder(string slug, Guid id) =>
            _context.Orders
                .Include(o => o.Product)!.ThenInclude(p => p!.Store)
                .FirstOrDefault(o => o.PublicId == id && o.Product!.Store!.Slug == slug);
    }
}
