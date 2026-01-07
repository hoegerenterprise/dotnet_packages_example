# .NET 8 Reusable Package Demo

This project demonstrates how to create reusable .NET 8 packages with database access that can be integrated into a main application.

## Project Structure

```
dotnet_packages/
├── src/
│   ├── MainApp/              # Main console application
│   │   ├── Models/
│   │   │   ├── Customer.cs
│   │   │   └── Order.cs
│   │   ├── Data/
│   │   │   └── AppDbContext.cs
│   │   ├── Program.cs
│   │   └── MainApp.csproj
│   └── MyPackage/            # Reusable package library
│       ├── Models/
│       │   └── Product.cs
│       ├── Data/
│       │   └── PackageDbInitializer.cs
│       └── MyPackage.csproj
└── README.md
```

## Key Concepts Demonstrated

### 1. Reusable Package Structure
- **MyPackage** is a class library that contains:
  - Database models (Product)
  - Data seeding logic (PackageDbInitializer)
  - Can be packaged as a NuGet package for distribution

### 2. Main Application with Its Own Models
- **MainApp** contains its own models:
  - **Customer** - Application-specific customer entity
  - **Order** - Links MainApp's Customer with MyPackage's Product
- Demonstrates how main app and package models work together in the same database

### 3. Database Integration
- The package provides models and seed data via `PackageDbInitializer.AddPackageData()`
- The main app's `AppDbContext` includes:
  - DbSets for both package models (Product) and app models (Customer, Order)
  - Seed data for main app models
  - Integration of package seed data through `PackageDbInitializer`
- The Order model demonstrates cross-package relationships (MainApp referencing MyPackage models)

### 4. Unified Database Access
- Single DbContext manages all models from both main app and package
- Entity relationships work seamlessly across package boundaries
- Both package and application data are seeded automatically on database creation

## How It Works

1. **Package defines models**: `MyPackage/Models/Product.cs` defines the Product entity
2. **Package provides seed data**: `MyPackage/Data/PackageDbInitializer.cs` contains 3 pre-configured products
3. **Main app defines its own models**: `Customer.cs` and `Order.cs` in MainApp/Models
4. **Main app references package**: MainApp.csproj includes a project reference to MyPackage
5. **Unified DbContext**: `AppDbContext` manages all entities:
   - Includes DbSets for Product (from package), Customer, and Order (from main app)
   - Seeds package data via `PackageDbInitializer.AddPackageData()`
   - Seeds main app data (2 customers, 2 orders linking to package products)
6. **Cross-package relationships**: Order entity references both Customer (MainApp) and Product (MyPackage)
7. **Database creation**: When the app runs, EF Core creates the database with all models and seed data

## Running the Demo

### Prerequisites
- .NET 8 SDK

### Build and Run

```bash
# Build the projects
dotnet build

# Run the main application
dotnet run

# Or run with project path
dotnet run --project src/MainApp
```

### Expected Output

```
=== .NET 8 Package Demo ===

Database created with models and data from both MainApp and MyPackage!

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
PRODUCTS (from MyPackage)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

#1 - Premium Widget
  Description: High-quality widget from package
  Price: $29.99
  Category: Widgets

#2 - Deluxe Gadget
  Description: Advanced gadget from package
  Price: $49.99
  Category: Gadgets

#3 - Basic Tool
  Description: Essential tool from package
  Price: $19.99
  Category: Tools

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
CUSTOMERS (from MainApp)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

#1 - John Doe
  Email: john.doe@example.com
  Registered: 2024-01-15

#2 - Jane Smith
  Email: jane.smith@example.com
  Registered: 2024-03-20

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
ORDERS (MainApp models referencing MyPackage models)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Order #1
  Customer: John Doe
  Product: Premium Widget (from package)
  Quantity: 2
  Total: $59.98
  Date: 2024-06-10

Order #2
  Customer: Jane Smith
  Product: Basic Tool (from package)
  Quantity: 1
  Total: $19.99
  Date: 2024-07-05

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
ADDING NEW DATA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

✓ Added new product: Super Gadget
✓ Added new customer: Alice Johnson
✓ Created order for Alice Johnson - Super Gadget

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
SUMMARY
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Total Products: 4 (3 from package + 1 added)
Total Customers: 3 (2 seeded + 1 added)
Total Orders: 3 (2 seeded + 1 added)

✓ Demo complete! Both MainApp and MyPackage models working together.
```

## Creating a NuGet Package

To distribute MyPackage as a reusable NuGet package:

```bash
cd src/MyPackage
dotnet pack -c Release
```

The package will be created in `bin/Release/MyPackage.1.0.0.nupkg`

## Installing the Package in Other Projects

### Option 1: Local Package
```bash
dotnet add package MyPackage --source /path/to/dotnet_packages/src/MyPackage/bin/Release
```

### Option 2: Publish to NuGet.org
```bash
dotnet nuget push bin/Release/MyPackage.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

Then install from NuGet:
```bash
dotnet add package MyPackage
```

## Extending the Demo

### Adding More Models to the Package
1. Create new entity classes in `MyPackage/Models/`
2. Add seed data in `PackageDbInitializer.AddPackageData()`
3. Add DbSet properties in `AppDbContext`

### Creating Multiple Packages
You can create multiple packages (e.g., MyPackage.Inventory, MyPackage.Orders) each with their own models and initializers, and combine them in the main app.

### Using Dependency Injection
For more advanced scenarios, you can:
- Add configuration options to the package
- Use dependency injection for DbContext
- Create service classes in the package
- Add migrations support

## Technologies Used

- .NET 8
- Entity Framework Core 8
- SQLite database
- C# 12

## License

This is a demonstration project for educational purposes.
