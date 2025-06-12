using Microsoft.EntityFrameworkCore;
using TeaShop.DataBase;

namespace TeaShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            string connection = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<Database>(options => options.UseSqlServer(connection));

            builder.Services.AddAuthentication("TeaShopCookie")
                .AddCookie("TeaShopCookie", options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Logout";
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                });

            builder.Services.AddSession();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.MapRazorPages();

            app.Run();
        }
    }
}
