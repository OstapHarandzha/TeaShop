using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TeaShop.DataBase;

namespace TeaShop.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly Database _db;

        public LogoutModel(Database db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var cart = HttpContext.Session.GetObjectFromJson<List<int>>("Cart") ?? new List<int>();
                var order = _db.Orders.Include(o => o.Items).FirstOrDefault(o => o.UserId == userId && o.IsActive);
                if (cart.Count > 0)
                {
                    if (order == null)
                    {
                        order = new TeaShop.Models.Order { UserId = userId, IsActive = true, Items = new List<TeaShop.Models.OrderItem>() };
                        _db.Orders.Add(order);
                    }
                    // Clear existing items
                    order.Items.Clear();
                    // Add items from session cart
                    foreach (var group in cart.GroupBy(x => x))
                    {
                        order.Items.Add(new TeaShop.Models.OrderItem { ProductId = group.Key, Quantity = group.Count() });
                    }
                    _db.SaveChanges();
                }
                else if (order != null)
                {
                    // If cart is empty, clear the active order
                    order.Items.Clear();
                    _db.SaveChanges();
                }
            }
            await HttpContext.SignOutAsync("TeaShopCookie");
            HttpContext.Session.Remove("Cart");
            return RedirectToPage("/Index");
        }
    }
} 