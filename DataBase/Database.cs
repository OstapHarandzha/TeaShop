using Microsoft.EntityFrameworkCore;
using TeaShop.Models;

namespace TeaShop.DataBase
{
    public class Database : DbContext
    {
        public Database(DbContextOptions<Database> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Seed admin user
            var admin = new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@teashop.com",
                Password = "admin1234", // Plain text for development
                IsAdmin = true
            };
            modelBuilder.Entity<User>().HasData(admin);
        }
    }
}
