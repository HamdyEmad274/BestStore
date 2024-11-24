using BestStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BestStore.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Product> Products { get; set; }
    }
}
