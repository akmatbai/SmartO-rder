using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartO_rder.Models;

namespace SmartO_rder.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Store> Stores { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<Cafe> Cafes { get; set; } = default!;
        public DbSet<Table> Tables { get; set; } = default!;
        public DbSet<Order> Orders { get; set; } = default!;

        public DbSet<Order> Orders { get; set; } = default!;

    }
}
