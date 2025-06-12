using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TeaShop.Models;
using TeaShop.DataBase;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;

namespace TeaShop.Pages
{
    public class LoginModel : PageModel
    {
        private readonly Database _db;
        public LoginModel(Database db) { _db = db; }

        [BindProperty]
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == Email);
            if (user == null || user.Password != Password)
            {
                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("IsAdmin", user.IsAdmin ? "True" : "False")
            };
            var identity = new ClaimsIdentity(claims, "TeaShopCookie");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("TeaShopCookie", principal);

            // Load user's active order into session cart
            var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.UserId == user.Id && o.IsActive);
            if (order != null)
            {
                var cart = order.Items.SelectMany(i => Enumerable.Repeat(i.ProductId, i.Quantity)).ToList();
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return RedirectToPage("/Index");
        }
    }
} 