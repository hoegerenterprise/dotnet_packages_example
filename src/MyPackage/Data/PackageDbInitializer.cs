using Microsoft.EntityFrameworkCore;
using MyPackage.Models;

namespace MyPackage.Data;

public static class PackageDbInitializer
{
    public static void AddPackageData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Premium Widget",
                Description = "High-quality widget from package",
                Price = 29.99m,
                Category = "Widgets"
            },
            new Product
            {
                Id = 2,
                Name = "Deluxe Gadget",
                Description = "Advanced gadget from package",
                Price = 49.99m,
                Category = "Gadgets"
            },
            new Product
            {
                Id = 3,
                Name = "Basic Tool",
                Description = "Essential tool from package",
                Price = 19.99m,
                Category = "Tools"
            }
        );
    }
}
