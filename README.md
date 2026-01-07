# .NET 8 Reusable Package Demo with REST API

This project demonstrates how to create reusable .NET 8 packages with database access, REST API controllers, and DTOs that can be integrated into a main application.

## Project Structure

```
dotnet_packages/
├── src/
│   ├── MainApp/              # Main Web API application
│   │   ├── Controllers/
│   │   │   ├── CustomersController.cs
│   │   │   └── OrdersController.cs
│   │   ├── DTOs/
│   │   │   ├── CustomerDto.cs
│   │   │   └── OrderDto.cs
│   │   ├── Models/
│   │   │   ├── Customer.cs
│   │   │   └── Order.cs
│   │   ├── Data/
│   │   │   └── AppDbContext.cs
│   │   ├── Program.cs
│   │   └── MainApp.csproj
│   └── MyPackage/            # Reusable package library with API controllers
│       ├── Controllers/
│       │   └── ProductsController.cs
│       ├── DTOs/
│       │   └── ProductDto.cs
│       ├── Models/
│       │   └── Product.cs
│       ├── Data/
│       │   └── PackageDbInitializer.cs
│       └── MyPackage.csproj
├── .gitignore
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

### 5. REST API with Controllers and DTOs
- **MainApp** is an ASP.NET Core Web API with:
  - `CustomersController` - Full CRUD operations for customers
  - `OrdersController` - Full CRUD operations for orders
  - DTOs (Data Transfer Objects) for clean API contracts
  - Swagger/OpenAPI documentation at root URL
- **MyPackage** includes:
  - `ProductsController` - Discovered via Application Parts
  - Product DTOs (ProductDto, CreateProductDto, UpdateProductDto)
  - Controllers can be reused across multiple applications
- **Controller Discovery**: MainApp automatically discovers and registers controllers from MyPackage
- **Dependency Injection**: DbContext is properly injected into controllers from both projects

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

# Run the Web API
dotnet run --project src/MainApp

# Or specify a custom port
dotnet run --project src/MainApp --urls "http://localhost:5123"
```

### Access the API

Once running, you can access:
- **Swagger UI**: http://localhost:5000/docs (interactive API documentation)
- **Products API**: http://localhost:5000/api/v1/products
- **Customers API**: http://localhost:5000/api/v1/customers
- **Orders API**: http://localhost:5000/api/v1/orders

### API Endpoints

#### Products (from MyPackage)
```bash
GET    /api/v1/products       # Get all products
GET    /api/v1/products/{id}  # Get product by ID
POST   /api/v1/products       # Create new product
PUT    /api/v1/products/{id}  # Update product
DELETE /api/v1/products/{id}  # Delete product
```

#### Customers (from MainApp)
```bash
GET    /api/v1/customers       # Get all customers
GET    /api/v1/customers/{id}  # Get customer by ID
POST   /api/v1/customers       # Create new customer
PUT    /api/v1/customers/{id}  # Update customer
DELETE /api/v1/customers/{id}  # Delete customer
```

#### Orders (from MainApp)
```bash
GET    /api/v1/orders       # Get all orders
GET    /api/v1/orders/{id}  # Get order by ID
POST   /api/v1/orders       # Create new order
PUT    /api/v1/orders/{id}  # Update order
DELETE /api/v1/orders/{id}  # Delete order
```

### Example API Requests

```bash
# Get all products
curl http://localhost:5000/api/v1/products

# Get a specific customer
curl http://localhost:5000/api/v1/customers/1

# Create a new order
curl -X POST http://localhost:5000/api/v1/orders \
  -H "Content-Type: application/json" \
  -d '{"customerId": 1, "productId": 2, "quantity": 3}'
```

### Expected API Responses

The database is automatically created with seed data. Here are example API responses:

**GET /api/v1/products**
```json
[
  {
    "id": 1,
    "name": "Premium Widget",
    "description": "High-quality widget from package",
    "price": 29.99,
    "category": "Widgets"
  },
  {
    "id": 2,
    "name": "Deluxe Gadget",
    "description": "Advanced gadget from package",
    "price": 49.99,
    "category": "Gadgets"
  },
  {
    "id": 3,
    "name": "Basic Tool",
    "description": "Essential tool from package",
    "price": 19.99,
    "category": "Tools"
  }
]
```

**GET /api/v1/customers**
```json
[
  {
    "id": 1,
    "name": "John Doe",
    "email": "john.doe@example.com",
    "registeredDate": "2024-01-15T00:00:00"
  },
  {
    "id": 2,
    "name": "Jane Smith",
    "email": "jane.smith@example.com",
    "registeredDate": "2024-03-20T00:00:00"
  }
]
```

**GET /api/v1/orders**
```json
[
  {
    "id": 1,
    "customerId": 1,
    "customerName": "John Doe",
    "productId": 1,
    "productName": "Premium Widget",
    "quantity": 2,
    "orderDate": "2024-06-10T00:00:00",
    "totalAmount": 59.98
  },
  {
    "id": 2,
    "customerId": 2,
    "customerName": "Jane Smith",
    "productId": 3,
    "productName": "Basic Tool",
    "quantity": 1,
    "orderDate": "2024-07-05T00:00:00",
    "totalAmount": 19.99
  }
]
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
- ASP.NET Core 8 (Web API)
- Entity Framework Core 8
- SQLite database
- Swagger/OpenAPI (Swashbuckle)
- DTOs (Data Transfer Objects)
- Application Parts (Controller Discovery)
- C# 12

## License

This is a demonstration project for educational purposes.
