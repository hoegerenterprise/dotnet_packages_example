# .NET 8 Package Structure Scaffolding Skill

This skill automates the creation of a .NET 8 solution with a reusable NuGet package and main application, both configured with Entity Framework Core.

## What This Skill Creates

- Complete .NET 8 solution with proper structure
- Reusable NuGet package with EF Core models and seed data
- Main application with its own models
- Cross-package relationships (app models referencing package models)
- Unified DbContext managing all entities
- Demo Program.cs showing the integration
- Comprehensive README documentation

## Usage

### Option 1: Via Claude Code

Simply ask Claude:
```
"Create a .NET package structure"
"Scaffold a new .NET 8 project with a reusable package"
"Set up a .NET solution with Entity Framework Core"
```

Claude will automatically use this skill and ask for:
- Solution name
- Package name
- Main app name
- Database provider (sqlite, sqlserver, postgres)

### Option 2: Direct Script Usage

```bash
cd /path/to/your/workspace
/path/to/.claude/skills/dotnet-package-scaffold/scripts/scaffold.sh MySolution MyPackage MyApp sqlite
```

Parameters:
1. Solution name (default: DemoApp)
2. Package name (default: SharedPackage)
3. App name (default: MainApp)
4. Database provider (default: sqlite)
   - `sqlite` - SQLite (best for demos)
   - `sqlserver` - SQL Server
   - `postgres` - PostgreSQL

## Generated Structure

```
YourSolution/
├── YourSolution.sln
├── src/
│   ├── MainApp/
│   │   ├── Models/
│   │   │   ├── Customer.cs
│   │   │   └── Order.cs
│   │   ├── Data/
│   │   │   └── AppDbContext.cs
│   │   ├── Program.cs
│   │   └── MainApp.csproj
│   └── YourPackage/
│       ├── Models/
│       │   └── Product.cs
│       ├── Data/
│       │   └── PackageDbInitializer.cs
│       └── YourPackage.csproj
└── README.md
```

## Key Features

### Package Models
- Generic, reusable entity models
- Data seeding through `PackageDbInitializer`
- Can be distributed as NuGet package

### Application Models
- Application-specific entities
- Can reference package models
- Demonstrates real-world usage patterns

### Unified DbContext
- Manages both package and app models
- Integrates seed data from both sources
- Shows proper EF Core configuration

### Cross-Package Relationships
- Example: Order (app model) references Product (package model)
- Demonstrates how to build on top of package entities

## After Scaffolding

1. Build the solution:
   ```bash
   dotnet build
   ```

2. Run the application:
   ```bash
   dotnet run --project src/MainApp
   ```

3. Package the library:
   ```bash
   cd src/YourPackage
   dotnet pack -c Release
   ```

4. Use the package in other projects:
   ```bash
   dotnet add package YourPackage --source /path/to/bin/Release
   ```

## Customization

The generated structure is a starting point. You can:
- Add more models to either the package or app
- Create multiple packages
- Change to Web API instead of console app
- Add migrations instead of using `EnsureCreated()`
- Implement dependency injection
- Add unit tests
- Create multiple applications using the same package

## Examples

### E-commerce System
```bash
./scaffold.sh EcommerceSystem Ecommerce.Products OrderManagement sqlite
```
- Package: Product catalog models
- App: Order management with customers and orders

### Multi-tenant SaaS
```bash
./scaffold.sh SaasApp Shared.Core TenantApi sqlserver
```
- Package: Shared core entities
- App: Tenant-specific API

### Microservices
Create multiple apps using the same package:
```bash
./scaffold.sh MicroservicesDemo Shared.Models InventoryService postgres
# Add another service
cd MicroservicesDemo/src
dotnet new webapi -n OrderService
dotnet add OrderService/OrderService.csproj reference Shared.Models/Shared.Models.csproj
```

## Benefits

1. **Consistency**: All projects follow the same structure
2. **Reusability**: Package can be shared across multiple applications
3. **Separation of Concerns**: Clear boundaries between shared and app-specific code
4. **Scalability**: Easy to add more packages or applications
5. **Best Practices**: Follows .NET conventions and EF Core patterns

## Troubleshooting

### Build Errors
- Ensure .NET 8 SDK is installed: `dotnet --version`
- Check all package references are added: `dotnet restore`

### Runtime Errors
- Verify database connection string in `AppDbContext`
- Check EF Core provider matches database type
- Ensure seed data IDs don't conflict

### Package Issues
- Make sure main app references the package project
- Rebuild solution after changing package models
- Check namespaces are correct in using statements

## Related Documentation

- [Microsoft: Create NuGet packages](https://learn.microsoft.com/en-us/nuget/create-packages/creating-a-package-dotnet-cli)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [.NET Solution Structure Best Practices](https://learn.microsoft.com/en-us/dotnet/core/porting/project-structure)

## Contributing

This skill is part of your project's `.claude/skills/` directory. Improvements and customizations are welcome!
