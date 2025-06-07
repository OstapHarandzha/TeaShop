using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TeaShop.Models;
using TeaShop.DataBase;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Security.Cryptography;
using System.Text;

namespace TeaShop.Pages
{
    public class SignupModel : PageModel
    {
        private readonly Database _db;
        public SignupModel(Database db) { _db = db; }

        [BindProperty]
        public SignupViewModel Signup { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Check for unique email (case-insensitive)
            if (await _db.Users.AnyAsync(u => u.Email.ToLower() == Signup.Email.ToLower()))
            {
                ModelState.AddModelError("Signup.Email", "An account with this email already exists.");
                return Page();
            }

            // Check for unique username
            if (await _db.Users.AnyAsync(u => u.Username == Signup.Username))
            {
                ModelState.AddModelError("Signup.Username", "This username is already taken.");
                return Page();
            }

            var user = new User
            {
                Username = Signup.Username,
                Email = Signup.Email,
                Password = Signup.Password
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // Sign in
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var identity = new ClaimsIdentity(claims, "TeaShopCookie");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("TeaShopCookie", principal);

            return RedirectToPage("/Index");
        }
    }
} 