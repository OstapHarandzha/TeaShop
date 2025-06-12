using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeaShop.Models;
using TeaShop.DataBase;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TeaShop.Pages
{
    public class CartModel : PageModel
    {
        private readonly Database _db;
        public CartModel(Database db) { _db = db; }

        public List<CartItem> Items { get; set; } = new();
        public decimal Total { get; set; }
        public List<Order> UserOrders { get; set; } = new();

        public async Task OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var order = _db.Orders.Include(o => o.Items).ThenInclude(i => i.Product).FirstOrDefault(o => o.UserId == userId && o.IsActive);
                if (order != null)
                {
                    Items = order.Items.Select(i => new CartItem { Product = i.Product, Quantity = i.Quantity }).ToList();
                    Total = Items.Sum(i => i.Product.Price * i.Quantity);
                    // Sync session cart for UI consistency
                    var cart = order.Items.SelectMany(i => Enumerable.Repeat(i.ProductId, i.Quantity)).ToList();
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                }
                // Load user's submitted orders (inactive)
                UserOrders = _db.Orders.Include(o => o.Items).ThenInclude(i => i.Product)
                    .Where(o => o.UserId == userId && !o.IsActive)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();
                return;
            }
            // Guest or no active order
            var sessionCart = HttpContext.Session.GetObjectFromJson<List<int>>("Cart") ?? new List<int>();
            var grouped = sessionCart.GroupBy(id => id).ToDictionary(g => g.Key, g => g.Count());
            var products = _db.Products.Where(p => grouped.Keys.Contains(p.Id)).ToList();
            Items = products.Select(p => new CartItem { Product = p, Quantity = grouped[p.Id] }).ToList();
            Total = Items.Sum(i => i.Product.Price * i.Quantity);
        }

        public IActionResult OnPostRemove(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var order = _db.Orders.Include(o => o.Items).FirstOrDefault(o => o.UserId == userId && o.IsActive);
                if (order != null)
                {
                    var item = order.Items.FirstOrDefault(i => i.ProductId == id);
                    if (item != null)
                    {
                        if (item.Quantity > 1)
                            item.Quantity--;
                        else
                            order.Items.Remove(item);
                        _db.SaveChanges();
                    }
                    // Sync session cart
                    var cart = order.Items.SelectMany(i => Enumerable.Repeat(i.ProductId, i.Quantity)).ToList();
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                }
                return RedirectToPage();
            }
            // Guest
            var sessionCart = HttpContext.Session.GetObjectFromJson<List<int>>("Cart") ?? new List<int>();
            sessionCart.Remove(id);
            HttpContext.Session.SetObjectAsJson("Cart", sessionCart);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSubmitOrderAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToPage("/Account/Login");
            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var order = _db.Orders.Include(o => o.Items).FirstOrDefault(o => o.UserId == userId && o.IsActive);
            if (order == null || order.Items.Count == 0)
                return RedirectToPage();
            order.IsActive = false;
            order.Status = "Pending";
            _db.SaveChanges();
            // Clear session cart
            HttpContext.Session.Remove("Cart");
            return RedirectToPage();
        }

        public class CartItem
        {
            public Product Product { get; set; }
            public int Quantity { get; set; }
        }
    }
} 