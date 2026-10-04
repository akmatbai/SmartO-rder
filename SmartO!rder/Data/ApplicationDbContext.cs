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
        public DbSet<MenuItem> MenuItems { get; set; } = default!;
        public DbSet<CafeOrder> CafeOrders { get; set; } = default!;
        public DbSet<CafeOrderItem> CafeOrderItems { get; set; } = default!;
        public DbSet<CafeStaff> CafeStaff { get; set; } = default!;

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
            builder.Entity<Order>().HasIndex(o => o.PublicId).IsUnique();
            builder.Entity<Order>().Property(o => o.UnitPrice).HasPrecision(18, 2);

            builder.Entity<MenuItem>().Property(m => m.Price).HasPrecision(18, 2);

            builder.Entity<CafeOrder>().HasIndex(o => o.PublicId).IsUnique();
            builder.Entity<CafeOrder>().HasOne(o => o.Cafe).WithMany()
                .HasForeignKey(o => o.CafeId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CafeOrder>().HasOne(o => o.Table).WithMany()
                .HasForeignKey(o => o.TableId).OnDelete(DeleteBehavior.SetNull);
            builder.Entity<CafeOrderItem>().HasOne(i => i.CafeOrder).WithMany(o => o.Items)
                .HasForeignKey(i => i.CafeOrderId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CafeOrderItem>().HasOne(i => i.MenuItem).WithMany()
                .HasForeignKey(i => i.MenuItemId).OnDelete(DeleteBehavior.SetNull);
            builder.Entity<CafeOrderItem>().Property(i => i.UnitPrice).HasPrecision(18, 2);

            builder.Entity<CafeStaff>().HasIndex(s => new { s.UserId, s.CafeId }).IsUnique();
        }

        // Cafés a Cook or Waiter is assigned to.
        public IQueryable<int> CafeIdsForStaff(string userId) =>
            CafeStaff.Where(s => s.UserId == userId).Select(s => s.CafeId);
    }
}
