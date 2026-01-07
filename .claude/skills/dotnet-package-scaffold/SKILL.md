---
name: dotnet-package-scaffold
description: Scaffolds a .NET 8 Web API solution with a main application and a reusable NuGet package library, both configured with Entity Framework Core, REST API controllers, DTOs, and Swagger documentation. Creates a complete project structure with models, controllers, and DTOs in both the main app and the package, demonstrating cross-package relationships and API versioning. Use when creating new .NET 8 API projects that need reusable packages with database integration.
---

# .NET 8 Package Structure with REST API and Entity Framework Core

## Purpose

This skill scaffolds a complete .NET 8 Web API solution demonstrating:
- A reusable NuGet package with EF Core models, controllers, DTOs, and seed data
- A main Web API application with its own models, controllers, and DTOs
- Cross-package relationships between app and package models
- Unified database context managing both app and package entities
- REST API controllers from both projects working together
- Swagger/OpenAPI documentation at `/docs`
- API versioning with `/api/v1/...` routes
- Proper project structure following .NET and REST API conventions

## When to Use

Use this skill when:
- Creating a new .NET 8 Web API with reusable packages
- Building REST APIs that share models and controllers across projects
- Setting up Entity Framework Core with multiple model sources
- Demonstrating package-based API architecture patterns
- User asks to "scaffold a .NET package structure" or "create a .NET API with packages"

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
│   │   ├── Controllers/
│   │   ├── DTOs/
│   │   ├── Models/
│   │   ├── Data/
│   │   ├── Program.cs
│   │   └── {MainAppName}.csproj
│   └── {PackageName}/
│       ├── Controllers/
│       ├── DTOs/
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
dotnet new webapi -o src/{MainAppName}
dotnet new classlib -o src/{PackageName}

# Add projects to solution
dotnet sln add src/{MainAppName}/{MainAppName}.csproj
dotnet sln add src/{PackageName}/{PackageName}.csproj

# Add EF Core packages to MainApp
cd src/{MainAppName}
dotnet add package Microsoft.EntityFrameworkCore.{provider} --version 8.0.*
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.*
dotnet add package Swashbuckle.AspNetCore
dotnet add package Microsoft.AspNetCore.OpenApi
dotnet add reference ../{PackageName}/{PackageName}.csproj

# Add packages to reusable package
cd ../{PackageName}
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.*
```

Replace `{provider}` with:
- `Sqlite` for SQLite
- `SqlServer` for SQL Server
- Use `Npgsql.EntityFrameworkCore.PostgreSQL` package for PostgreSQL

### Step 4: Configure Package for Controllers

Update `{PackageName}.csproj` to support ASP.NET Core:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.*" />
  </ItemGroup>
</Project>
```

### Step 5: Create Package Models and DTOs

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

#### Package DTOs (`src/{PackageName}/DTOs/ProductDto.cs`)

```csharp
namespace {PackageName}.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class UpdateProductDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? Category { get; set; }
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

#### Package Controller (`src/{PackageName}/Controllers/ProductsController.cs`)

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using {PackageName}.Models;
using {PackageName}.DTOs;

namespace {PackageName}.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly DbContext _context;

    public ProductsController(DbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _context.Set<Product>()
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Category = p.Category
            })
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _context.Set<Product>().FindAsync(id);

        if (product == null)
            return NotFound();

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Category = product.Category
        };

        return Ok(productDto);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createDto)
    {
        var product = new Product
        {
            Name = createDto.Name,
            Description = createDto.Description,
            Price = createDto.Price,
            Category = createDto.Category
        };

        _context.Set<Product>().Add(product);
        await _context.SaveChangesAsync();

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Category = product.Category
        };

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updateDto)
    {
        var product = await _context.Set<Product>().FindAsync(id);

        if (product == null)
            return NotFound();

        if (updateDto.Name != null)
            product.Name = updateDto.Name;
        if (updateDto.Description != null)
            product.Description = updateDto.Description;
        if (updateDto.Price.HasValue)
            product.Price = updateDto.Price.Value;
        if (updateDto.Category != null)
            product.Category = updateDto.Category;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Set<Product>().FindAsync(id);

        if (product == null)
            return NotFound();

        _context.Set<Product>().Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
```

### Step 6: Create Main App Models and DTOs

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

#### Customer DTOs (`src/{MainAppName}/DTOs/CustomerDto.cs`)

```csharp
namespace {MainAppName}.DTOs;

public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime RegisteredDate { get; set; }
}

public class CreateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class UpdateCustomerDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
}
```

#### Order DTOs (`src/{MainAppName}/DTOs/OrderDto.cs`)

```csharp
namespace {MainAppName}.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
}

public class CreateOrderDto
{
    public int CustomerId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateOrderDto
{
    public int? CustomerId { get; set; }
    public int? ProductId { get; set; }
    public int? Quantity { get; set; }
}
```

### Step 7: Create Main App Controllers

#### Customers Controller (`src/{MainAppName}/Controllers/CustomersController.cs`)

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using {MainAppName}.Data;
using {MainAppName}.Models;
using {MainAppName}.DTOs;

namespace {MainAppName}.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _context;

    public CustomersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
    {
        var customers = await _context.Customers
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                RegisteredDate = c.RegisteredDate
            })
            .ToListAsync();

        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
            return NotFound();

        var customerDto = new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            RegisteredDate = customer.RegisteredDate
        };

        return Ok(customerDto);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> CreateCustomer(CreateCustomerDto createDto)
    {
        var customer = new Customer
        {
            Name = createDto.Name,
            Email = createDto.Email,
            RegisteredDate = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        var customerDto = new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            RegisteredDate = customer.RegisteredDate
        };

        return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customerDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, UpdateCustomerDto updateDto)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
            return NotFound();

        if (updateDto.Name != null)
            customer.Name = updateDto.Name;
        if (updateDto.Email != null)
            customer.Email = updateDto.Email;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
            return NotFound();

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
```

#### Orders Controller (`src/{MainAppName}/Controllers/OrdersController.cs`)

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using {MainAppName}.Data;
using {MainAppName}.Models;
using {MainAppName}.DTOs;

namespace {MainAppName}.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Product)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer.Name,
                ProductId = o.ProductId,
                ProductName = o.Product.Name,
                Quantity = o.Quantity,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount
            })
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return NotFound();

        var orderDto = new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer.Name,
            ProductId = order.ProductId,
            ProductName = order.Product.Name,
            Quantity = order.Quantity,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount
        };

        return Ok(orderDto);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createDto)
    {
        var customer = await _context.Customers.FindAsync(createDto.CustomerId);
        var product = await _context.Set<{PackageName}.Models.Product>().FindAsync(createDto.ProductId);

        if (customer == null || product == null)
            return BadRequest("Invalid customer or product ID");

        var order = new Order
        {
            CustomerId = createDto.CustomerId,
            ProductId = createDto.ProductId,
            Quantity = createDto.Quantity,
            OrderDate = DateTime.UtcNow,
            TotalAmount = product.Price * createDto.Quantity
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        await _context.Entry(order).Reference(o => o.Customer).LoadAsync();
        await _context.Entry(order).Reference(o => o.Product).LoadAsync();

        var orderDto = new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer.Name,
            ProductId = order.ProductId,
            ProductName = order.Product.Name,
            Quantity = order.Quantity,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount
        };

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, UpdateOrderDto updateDto)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
            return NotFound();

        if (updateDto.CustomerId.HasValue)
        {
            var customer = await _context.Customers.FindAsync(updateDto.CustomerId.Value);
            if (customer == null)
                return BadRequest("Invalid customer ID");
            order.CustomerId = updateDto.CustomerId.Value;
        }

        if (updateDto.ProductId.HasValue)
        {
            var product = await _context.Set<{PackageName}.Models.Product>().FindAsync(updateDto.ProductId.Value);
            if (product == null)
                return BadRequest("Invalid product ID");
            order.ProductId = updateDto.ProductId.Value;
        }

        if (updateDto.Quantity.HasValue)
        {
            order.Quantity = updateDto.Quantity.Value;
            var product = await _context.Set<{PackageName}.Models.Product>().FindAsync(order.ProductId);
            if (product != null)
                order.TotalAmount = product.Price * order.Quantity;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
            return NotFound();

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
```

### Step 8: Create Unified DbContext

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

### Step 9: Configure Program.cs with Swagger

Update the main app's Program.cs (`src/{MainAppName}/Program.cs`):

```csharp
using {MainAppName}.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddApplicationPart(typeof({PackageName}.Controllers.ProductsController).Assembly); // Discover controllers from package

// Add DbContext
builder.Services.AddDbContext<AppDbContext>();
// Register DbContext so package controllers can use it
builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<AppDbContext>());

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() {
        Title = "{MainAppName} API",
        Version = "v1",
        Description = "API demonstrating reusable packages with Entity Framework Core. Includes controllers from both {MainAppName} and {PackageName}."
    });
});

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "{MainAppName} API v1");
    c.RoutePrefix = "docs"; // Serve Swagger UI at /docs
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Logger.LogInformation("API started successfully!");
app.Logger.LogInformation("Swagger UI: http://localhost:5000/docs");
app.Logger.LogInformation("API Base: http://localhost:5000/api/v1");
app.Logger.LogInformation("Controllers:");
app.Logger.LogInformation("  - CustomersController ({MainAppName})");
app.Logger.LogInformation("  - OrdersController ({MainAppName})");
app.Logger.LogInformation("  - ProductsController ({PackageName})");

app.Run();
```

### Step 10: Create README

Generate a comprehensive README.md documenting:
- Project structure with controllers and DTOs
- Key concepts (package models, app models, cross-package relationships, API controllers)
- How to build and run the API
- Swagger documentation access at `/docs`
- API endpoint documentation with `/api/v1/...` routes
- Example curl commands
- How to package the library with `dotnet pack`
- How to extend the structure

### Step 11: Build and Test

```bash
# Build solution
dotnet build

# Run the Web API
dotnet run --project src/{MainAppName}

# Access Swagger UI
# Open browser to: http://localhost:5000/docs

# Test API endpoints
curl http://localhost:5000/api/v1/products
curl http://localhost:5000/api/v1/customers
curl http://localhost:5000/api/v1/orders
```

## Key Architecture Points to Explain

When scaffolding is complete, explain to the user:

1. **Package Isolation**: The package (`{PackageName}`) contains reusable models, DTOs, controllers, and data seeding logic
2. **App Models**: The main app has its own models (Customer, Order), DTOs, and controllers specific to the application
3. **Cross-Package Relationships**: Order model demonstrates linking app models to package models
4. **Controller Discovery**: Package controllers are discovered via Application Parts
5. **Unified DbContext**: Single context manages all entities from both sources
6. **Data Seeding**: Both package and app contribute seed data through the DbContext
7. **API Versioning**: All routes use `/api/v1/...` for future version compatibility
8. **DTOs**: Clean separation between database entities and API contracts
9. **Swagger Documentation**: Interactive API docs at `/docs` endpoint
10. **NuGet Distribution**: The package (with controllers!) can be distributed via `dotnet pack`

## API Best Practices Demonstrated

1. **Versioned Routes**: `/api/v1/[controller]` allows for future API versions
2. **Swagger at /docs**: Industry standard location for API documentation
3. **DTOs for API Contracts**: Separate request/response models from database entities
4. **RESTful Conventions**: Proper HTTP verbs and status codes
5. **Dependency Injection**: DbContext injected into controllers
6. **Cross-Package Controllers**: Demonstrates controller reusability

## Customization Options

The skill should be flexible. Ask if the user wants:
- Different model names/types
- Additional relationships
- Different database providers
- Migrations instead of EnsureCreated
- Authentication/Authorization
- Multiple packages
- Different API versioning strategies

## Example Usage

```
User: "Create a .NET API package structure"
↓
Claude uses this skill:
  1. Asks for solution/package names
  2. Creates directory structure with Controllers and DTOs folders
  3. Scaffolds both projects with models, DTOs, and controllers
  4. Sets up EF Core integration
  5. Configures Swagger at /docs
  6. Creates versioned API routes
  7. Generates comprehensive README
↓
Result: Complete, buildable Web API with package controllers working together
```

## Troubleshooting

### Build Errors
- Ensure .NET 8 SDK is installed: `dotnet --version`
- Check all package references are added: `dotnet restore`
- Verify package .csproj has `<FrameworkReference Include="Microsoft.AspNetCore.App" />`

### Runtime Errors
- Verify database connection string in `AppDbContext`
- Check EF Core provider matches database type
- Ensure seed data IDs don't conflict
- Verify DbContext is registered for dependency injection

### Controller Issues
- Ensure Application Parts is configured in Program.cs
- Check controllers use `[ApiController]` attribute
- Verify routes use `api/v1/[controller]`
- Ensure both DbContext and AppDbContext are registered

### Swagger Issues
- Verify Swashbuckle.AspNetCore is installed
- Check route prefix is set to "docs"
- Ensure UseSwagger() is called before UseSwaggerUI()

## Success Criteria

The scaffolding is successful when:
- `dotnet build` completes without errors
- `dotnet run --project src/{MainAppName}` starts the API
- Swagger UI loads at `http://localhost:5000/docs`
- All API endpoints respond at `/api/v1/...` routes
- Controllers from both projects are discoverable
- Cross-package relationships work (Order references Product)
- README accurately documents the API structure
- Package can be packed with `dotnet pack`
