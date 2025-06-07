using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeaShop.Models;
using TeaShop.DataBase;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeaShop.Pages
{
    public class AdminOrdersModel : PageModel
    {
        private readonly Database _db;
        public AdminOrdersModel(Database db) { _db = db; }

        public List<Order> Orders { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated || !IsAdmin())
                return Forbid();
            Orders = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Where(o => !o.IsActive)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostChangeStatusAsync(int id, string status)
        {
            if (!User.Identity.IsAuthenticated || !IsAdmin())
                return Forbid();
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order != null)
            {
                order.Status = status;
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        private bool IsAdmin()
        {
            return User.Claims.Any(c => c.Type == "IsAdmin" && c.Value == "True") || User.Identity.Name == "admin";
        }
    }
} 