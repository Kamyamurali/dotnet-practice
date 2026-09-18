using CoffeeShop.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Data;

// The DbContext IS your database, expressed in C#.
// Each DbSet<T> becomes a table; each property of T becomes a column.
public class CoffeeShopContext : DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    // Tells EF which database to talk to and how to connect.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // SQLite: a single file "coffeeshop.db" is created next to the app. No install needed.
        options.UseSqlite("Data Source=coffeeshop.db");

        // --- To use SQL Server instead (matches the T-SQL work in class), ---
        // --- comment the line above and uncomment the one below, then swap ---
        // --- the package in the .csproj to Microsoft.EntityFrameworkCore.SqlServer ---
        // options.UseSqlServer("Server=localhost;Database=CoffeeShop;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    // The Fluent API: fine-tune the model beyond what conventions give you.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Money columns: 10 digits total, 2 after the decimal.
        modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(10, 2);
        modelBuilder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasPrecision(10, 2);

        // Seed data: baked into the migration so a fresh DB starts with a menu.
        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, Name = "Hot Coffee" },
            new Category { CategoryId = 2, Name = "Cold Coffee" },
            new Category { CategoryId = 3, Name = "Pastries" }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, Name = "Espresso",   Price = 2.50m, CategoryId = 1 },
            new Product { ProductId = 2, Name = "Cappuccino", Price = 3.50m, CategoryId = 1 },
            new Product { ProductId = 3, Name = "Iced Latte", Price = 4.00m, CategoryId = 2 },
            new Product { ProductId = 4, Name = "Croissant",  Price = 2.75m, CategoryId = 3 }
        );
    }
}
