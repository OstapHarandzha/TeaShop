using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeaShop.Models;
using TeaShop.DataBase;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Linq;

namespace TeaShop.Pages
{
    public class AddProductModel : PageModel
    {
        private readonly Database _db;
        private readonly IWebHostEnvironment _env;
        public AddProductModel(Database db, IWebHostEnvironment env) { _db = db; _env = env; }

        [BindProperty]
        public Product Product { get; set; }
        [BindProperty]
        public IFormFile ImageFile { get; set; }
        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (!User.Identity.IsAuthenticated || !IsAdmin())
                return Forbid();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.Identity.IsAuthenticated || !IsAdmin())
                return Forbid();

            ModelState.Remove("Product.ImagePath");
            
            if (!ModelState.IsValid)
            {
                // Collect all field errors
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => $"{x.Key.Replace("Product.", "")} - {string.Join(", ", x.Value.Errors.Select(e => e.ErrorMessage))}")
                    .ToList();
                ErrorMessage = "Please correct the following fields: " + string.Join("; ", errors);
                return Page();
            }
            if (ImageFile == null || ImageFile.Length == 0)
            {
                ErrorMessage = "Please upload an image.";
                return Page();
            }
            // Save image
            var folder = Path.Combine(_env.WebRootPath, "shop-pictures");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
            var filePath = Path.Combine(folder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(stream);
            }
            Product.ImagePath = "/shop-pictures/" + fileName;
            _db.Products.Add(Product);
            await _db.SaveChangesAsync();
            return RedirectToPage("/Shop");
        }

        private bool IsAdmin()
        {
            return User.Claims.Any(c => c.Type == "IsAdmin" && c.Value == "True") || User.Identity.Name == "admin";
        }
    }
} 