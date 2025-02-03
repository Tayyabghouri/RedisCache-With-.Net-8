using Microsoft.EntityFrameworkCore;
using RedisCache.Models;

namespace RedisCache.Data
{ 

    public class AppDbContext:DbContext
    {
        public DbSet<Driver> Drivers { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }

    }
}

