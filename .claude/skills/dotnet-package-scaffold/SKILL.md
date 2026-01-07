---
name: dotnet-package-scaffold
description: Scaffolds a .NET 8 solution with a main application and a reusable NuGet package library, both configured with Entity Framework Core for database access. Creates a complete project structure with models in both the main app and the package, demonstrating cross-package relationships. Use when creating new .NET 8 projects that need reusable packages with database integration.
---

# .NET 8 Package Structure with Entity Framework Core

## Purpose

This skill scaffolds a complete .NET 8 solution demonstrating:
- A reusable NuGet package with EF Core models and seed data
- A main application with its own models
- Cross-package relationships between app and package models
- Unified database context managing both app and package entities
- Proper project structure following .NET conventions

## When to Use

Use this skill when:
- Creating a new .NET 8 solution with reusable packages
- Building applications that share models across projects via packages
- Setting up Entity Framework Core with multiple model sources
- Demonstrating package-based architecture patterns
- User asks to "scaffold a .NET package structure" or similar

## Instructions

### Step 1: Gather Project Information

Ask the user for:
1. **Solution name** (default: `DemoApp`)
2. **Package name** (default: `SharedPackage`)
3. **Main app name** (default: `MainApp`)
4. **Database provider**:
   - `sqlite` (default, easiest for demos)
   - `sqlserver` for SQL Server
   - `postgres` for PostgreSQL

### Step 2: Create Directory Structure

Create the following structure:
```
{solution-root}/
├── {solution-name}.sln
├── src/
│   ├── {MainAppName}/
│   │   ├── Models/
│   │   ├── Data/
│   │   ├── Program.cs
│   │   └── {MainAppName}.csproj
│   └── {PackageName}/
│       ├── Models/
│       ├── Data/
│       └── {PackageName}.csproj
└── README.md
```

### Step 3: Execute Scaffolding Commands

Run the following commands in sequence:

```bash
# Create solution file
dotnet new sln -n {solution-name}

# Create projects
mkdir -p src/{MainAppName} src/{PackageName}
dotnet new console -o src/{MainAppName}
dotnet new classlib -o src/{PackageName}

# Add projects to solution
dotnet sln add src/{MainAppName}/{MainAppName}.csproj
dotnet sln add src/{PackageName}/{PackageName}.csproj

# Add EF Core packages
cd src/{MainAppName}
dotnet add package Microsoft.EntityFrameworkCore.{provider} --version 8.0.*
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.*
dotnet add reference ../{PackageName}/{PackageName}.csproj

cd ../{PackageName}
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.*
```

Replace `{provider}` with:
- `Sqlite` for SQLite
- `SqlServer` for SQL Server
- Use `Npgsql.EntityFrameworkCore.PostgreSQL` package for PostgreSQL

### Step 4: Create Package Models and Data Initializer

Create the following files in the package:

#### Package Model (`src/{PackageName}/Models/Product.cs`)

```csharp
namespace {PackageName}.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
}
```

#### Package Data Initializer (`src/{PackageName}/Data/PackageDbInitializer.cs`)

```csharp
using Microsoft.EntityFrameworkCore;
using {PackageName}.Models;

namespace {PackageName}.Data;

public static class PackageDbInitializer
{
    public static void AddPackageData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Sample Product 1",
                Description = "Product from package",
                Price = 29.99m,
                Category = "Category A"
            },
            new Product
            {
                Id = 2,
                Name = "Sample Product 2",
                Description = "Another product from package",
                Price = 49.99m,
                Category = "Category B"
            },
            new Product
            {
                Id = 3,
                Name = "Sample Product 3",
                Description = "Third product from package",
                Price = 19.99m,
                Category = "Category A"
            }
        );
    }
}
```

### Step 5: Create Main App Models

Create application-specific models:

#### Customer Model (`src/{MainAppName}/Models/Customer.cs`)

```csharp
namespace {MainAppName}.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime RegisteredDate { get; set; }
}
```

#### Order Model (`src/{MainAppName}/Models/Order.cs`)

```csharp
using {PackageName}.Models;

namespace {MainAppName}.Models;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
}
```

Note: Order demonstrates cross-package relationship by referencing Product from the package.

### Step 6: Create Unified DbContext

Create the DbContext in the main app (`src/{MainAppName}/Data/AppDbContext.cs`):

```csharp
using Microsoft.EntityFrameworkCore;
using {PackageName}.Models;
using {PackageName}.Data;
using {MainAppName}.Models;

namespace {MainAppName}.Data;

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
        // For SQL Server: optionsBuilder.UseSqlServer("YourConnectionString");
        // For PostgreSQL: optionsBuilder.UseNpgsql("YourConnectionString");
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
                ProductId = 1,
                Quantity = 2,
                OrderDate = new DateTime(2024, 6, 10),
                TotalAmount = 59.98m
            },
            new Order
            {
                Id = 2,
                CustomerId = 2,
                ProductId = 3,
                Quantity = 1,
                OrderDate = new DateTime(2024, 7, 5),
                TotalAmount = 19.99m
            }
        );
    }
}
```

### Step 7: Create Program.cs

Update the main app's Program.cs (`src/{MainAppName}/Program.cs`):

```csharp
using {MainAppName}.Data;
using {MainAppName}.Models;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("=== .NET 8 Package Demo ===\n");

// Create and initialize database
using var db = new AppDbContext();
db.Database.EnsureDeleted();
db.Database.EnsureCreated();

Console.WriteLine("Database created with models from both MainApp and Package!\n");

// Query products from package
Console.WriteLine("PRODUCTS (from Package)");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━\n");

var products = await db.Products.ToListAsync();
foreach (var product in products)
{
    Console.WriteLine($"#{product.Id} - {product.Name}");
    Console.WriteLine($"  Price: ${product.Price:F2} | Category: {product.Category}");
}

// Query customers from main app
Console.WriteLine("\nCUSTOMERS (from MainApp)");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━\n");

var customers = await db.Customers.ToListAsync();
foreach (var customer in customers)
{
    Console.WriteLine($"#{customer.Id} - {customer.Name} ({customer.Email})");
}

// Query orders showing cross-package relationships
Console.WriteLine("\nORDERS (MainApp models referencing Package models)");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━\n");

var orders = await db.Orders
    .Include(o => o.Customer)
    .Include(o => o.Product)
    .ToListAsync();

foreach (var order in orders)
{
    Console.WriteLine($"Order #{order.Id}: {order.Customer.Name} ordered {order.Quantity}x {order.Product.Name}");
    Console.WriteLine($"  Total: ${order.TotalAmount:F2}");
}

Console.WriteLine($"\n✓ Demo complete! {await db.Products.CountAsync()} products, {await db.Customers.CountAsync()} customers, {await db.Orders.CountAsync()} orders.");
```

### Step 8: Create README

Generate a comprehensive README.md documenting:
- Project structure
- Key concepts (package models, app models, cross-package relationships)
- How to build and run
- How to package the library with `dotnet pack`
- How to extend the structure

### Step 9: Build and Test

```bash
# Build solution
dotnet build

# Run the application
dotnet run --project src/{MainAppName}
```

## Key Architecture Points to Explain

When scaffolding is complete, explain to the user:

1. **Package Isolation**: The package (`{PackageName}`) contains reusable models and data seeding logic
2. **App Models**: The main app has its own models (Customer, Order) specific to the application
3. **Cross-Package Relationships**: Order model demonstrates linking app models to package models
4. **Unified DbContext**: Single context manages all entities from both sources
5. **Data Seeding**: Both package and app contribute seed data through the DbContext
6. **NuGet Distribution**: The package can be distributed via `dotnet pack` and installed in other projects

## Customization Options

The skill should be flexible. Ask if the user wants:
- Different model names/types
- Additional relationships
- Web API instead of console app
- Migrations instead of EnsureCreated
- Different database providers
- Multiple packages

## Best Practices

1. Keep package models generic and reusable
2. App-specific logic stays in the main app
3. Use dependency injection for DbContext in production apps
4. Version packages with semantic versioning
5. Document cross-package dependencies clearly
6. Test package independently before publishing

## Example Usage

```
User: "Create a .NET package structure for me"
↓
Claude uses this skill:
  1. Asks for solution/package names
  2. Creates directory structure
  3. Scaffolds both projects with models
  4. Sets up EF Core integration
  5. Creates demo Program.cs
  6. Generates README
↓
Result: Complete, buildable solution with package and app working together
```

## Troubleshooting

- **Build errors**: Ensure all package references are added correctly
- **EF Core version mismatch**: Use consistent 8.0.* versions across all projects
- **Cross-package references not resolving**: Check that MainApp references the Package project
- **Database not creating**: Verify connection string and database provider packages

## Success Criteria

The scaffolding is successful when:
- `dotnet build` completes without errors
- `dotnet run --project src/{MainAppName}` executes and displays data
- Both package models and app models are queryable
- Cross-package relationships work (Order references Product)
- README accurately documents the structure
- Package can be packed with `dotnet pack`
