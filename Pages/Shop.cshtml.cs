using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeaShop.Models;
using TeaShop.DataBase;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace TeaShop.Pages
{
    public class ShopModel : PageModel
    {
        private readonly Database _db;
        private readonly IWebHostEnvironment _env;
        public ShopModel(Database db, IWebHostEnvironment env) { _db = db; _env = env; }

        public List<Product> Products { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        public async Task OnGetAsync()
        {
            var query = _db.Products.AsQueryable();
            if (!string.IsNullOrWhiteSpace(Search))
            {
                query = query.Where(p => p.Name.Contains(Search));
            }
            Products = await query.ToListAsync();
        }

        public IActionResult OnPostAddToCart(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Redirect to login if not authenticated
                return RedirectToPage("/Login");
            }

            // Get user ID
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            // Find active order
            var order = _db.Orders.Include(o => o.Items).FirstOrDefault(o => o.UserId == userId && o.IsActive);
            if (order == null)
            {
                order = new Order { UserId = userId, IsActive = true, Items = new List<OrderItem>() };
                _db.Orders.Add(order);
            }
            // Check if item already in cart
            var item = order.Items.FirstOrDefault(i => i.ProductId == id);
            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                order.Items.Add(new OrderItem { ProductId = id, Quantity = 1 });
            }
            _db.SaveChanges();

            // Sync session cart for UI consistency
            var cart = order.Items.SelectMany(i => Enumerable.Repeat(i.ProductId, i.Quantity)).ToList();
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            if (!User.Identity.IsAuthenticated || !User.Claims.Any(c => c.Type == "IsAdmin" && c.Value == "True"))
                return Forbid();
            var product = _db.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                // Remove image file
                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    var filePath = Path.Combine(_env.WebRootPath, product.ImagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }
                _db.Products.Remove(product);
                _db.SaveChanges();
            }
            return RedirectToPage();
        }
    }

    // Session helpers
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));
        }
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : System.Text.Json.JsonSerializer.Deserialize<T>(value);
        }
    }
} 