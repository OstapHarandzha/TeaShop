using Microsoft.EntityFrameworkCore;
using TeaShop.Models;

namespace TeaShop.DataBase
{
    public class Database : DbContext
    {
        public Database(DbContextOptions<Database> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
