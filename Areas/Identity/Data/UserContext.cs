using BookShop.Areas.Identity.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Areas.Identity.Data;

public class UserContext : IdentityDbContext<BookShopUser>
{
    public UserContext(DbContextOptions<UserContext> options)
        : base(options)
    {

    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Cart> Carts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.Entity<Book>()
               .HasOne<BookShopUser>(b => b.User)
               .WithMany(u => u.Books)
               .HasForeignKey(b => b.UId);

        builder.Entity<Book>()
            .HasOne<Category>(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId);

        builder.Entity<Order>()
            .HasOne<BookShopUser>(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UId);

        builder.Entity<OrderDetail>()
            .HasKey(od => new { od.OrderId, od.BookIsbn });

        builder.Entity<OrderDetail>()
            .HasOne<Order>(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.NoAction);


        builder.Entity<OrderDetail>()
            .HasOne<Book>(od => od.Book)
            .WithMany(b => b.OrderDetails)
            .HasForeignKey(od => od.BookIsbn)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Cart>()
            .HasKey(c => new { c.UId, c.BookIsbn });

        builder.Entity<Cart>()
            .HasOne<BookShopUser>(c => c.User)
            .WithMany(u => u.Carts)
            .HasForeignKey(c => c.UId);

        builder.Entity<Cart>()
            .HasOne<Book>(c => c.Book)
            .WithMany(b => b.Carts)
            .HasForeignKey(c => c.BookIsbn)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
