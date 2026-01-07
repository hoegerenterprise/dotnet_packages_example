using Microsoft.EntityFrameworkCore;
using MyPackage.Models;
using MyPackage.Data;
using MainApp.Models;

namespace MainApp.Data;

public class AppDbContext : DbContext
{
    // Models from the package
    public DbSet<Product> Products { get; set; }

    // Models from the main app
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=app.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Add data from the package
        PackageDbInitializer.AddPackageData(modelBuilder);

        // Add seed data for main app models
        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                RegisteredDate = new DateTime(2024, 1, 15)
            },
            new Customer
            {
                Id = 2,
                Name = "Jane Smith",
                Email = "jane.smith@example.com",
                RegisteredDate = new DateTime(2024, 3, 20)
            }
        );

        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                Id = 1,
                CustomerId = 1,
                ProductId = 1, // Premium Widget from package
                Quantity = 2,
                OrderDate = new DateTime(2024, 6, 10),
                TotalAmount = 59.98m
            },
            new Order
            {
                Id = 2,
                CustomerId = 2,
                ProductId = 3, // Basic Tool from package
                Quantity = 1,
                OrderDate = new DateTime(2024, 7, 5),
                TotalAmount = 19.99m
            }
        );
    }
}
