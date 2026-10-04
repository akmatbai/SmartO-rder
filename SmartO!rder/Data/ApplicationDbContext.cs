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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Store>().HasIndex(s => s.Slug).IsUnique();
            builder.Entity<Cafe>().HasIndex(c => c.Slug).IsUnique();
            builder.Entity<Table>().HasIndex(t => new { t.CafeId, t.Number }).IsUnique();
            builder.Entity<Product>().HasIndex(p => new { p.StoreId, p.Article }).IsUnique();
            builder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);

            // Deleting a user or product must not silently wipe stores, cafés or order history.
            builder.Entity<Store>().HasOne(s => s.Owner).WithMany()
                .HasForeignKey(s => s.OwnerId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Cafe>().HasOne(c => c.Owner).WithMany()
                .HasForeignKey(c => c.OwnerId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Order>().HasOne(o => o.User).WithMany()
                .HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Order>().HasOne(o => o.Product).WithMany()
                .HasForeignKey(o => o.ProductId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
