using MainApp.Data;
using MainApp.Models;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("=== .NET 8 Package Demo ===\n");

// Create and migrate the database
using var db = new AppDbContext();
db.Database.EnsureDeleted();
db.Database.EnsureCreated();

Console.WriteLine("Database created with models and data from both MainApp and MyPackage!\n");

// ===== PRODUCTS FROM PACKAGE =====
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine("PRODUCTS (from MyPackage)");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

var products = await db.Products.ToListAsync();

foreach (var product in products)
{
    Console.WriteLine($"#{product.Id} - {product.Name}");
    Console.WriteLine($"  Description: {product.Description}");
    Console.WriteLine($"  Price: ${product.Price:F2}");
    Console.WriteLine($"  Category: {product.Category}");
    Console.WriteLine();
}

// ===== CUSTOMERS FROM MAIN APP =====
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine("CUSTOMERS (from MainApp)");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

var customers = await db.Customers.ToListAsync();

foreach (var customer in customers)
{
    Console.WriteLine($"#{customer.Id} - {customer.Name}");
    Console.WriteLine($"  Email: {customer.Email}");
    Console.WriteLine($"  Registered: {customer.RegisteredDate:yyyy-MM-dd}");
    Console.WriteLine();
}

// ===== ORDERS - LINKING MAIN APP AND PACKAGE MODELS =====
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine("ORDERS (MainApp models referencing MyPackage models)");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

var orders = await db.Orders
    .Include(o => o.Customer)
    .Include(o => o.Product)
    .ToListAsync();

foreach (var order in orders)
{
    Console.WriteLine($"Order #{order.Id}");
    Console.WriteLine($"  Customer: {order.Customer.Name}");
    Console.WriteLine($"  Product: {order.Product.Name} (from package)");
    Console.WriteLine($"  Quantity: {order.Quantity}");
    Console.WriteLine($"  Total: ${order.TotalAmount:F2}");
    Console.WriteLine($"  Date: {order.OrderDate:yyyy-MM-dd}");
    Console.WriteLine();
}

// ===== ADDING NEW DATA DYNAMICALLY =====
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine("ADDING NEW DATA");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

// Add a new product from main app
var newProduct = new MyPackage.Models.Product
{
    Name = "Super Gadget",
    Description = "Added dynamically by main application",
    Price = 99.99m,
    Category = "Premium"
};

db.Products.Add(newProduct);
await db.SaveChangesAsync();

Console.WriteLine($"✓ Added new product: {newProduct.Name}");

// Add a new customer from main app
var newCustomer = new Customer
{
    Name = "Alice Johnson",
    Email = "alice.johnson@example.com",
    RegisteredDate = DateTime.Now
};

db.Customers.Add(newCustomer);
await db.SaveChangesAsync();

Console.WriteLine($"✓ Added new customer: {newCustomer.Name}");

// Create an order linking the new customer and new product
var newOrder = new Order
{
    CustomerId = newCustomer.Id,
    ProductId = newProduct.Id,
    Quantity = 1,
    OrderDate = DateTime.Now,
    TotalAmount = newProduct.Price
};

db.Orders.Add(newOrder);
await db.SaveChangesAsync();

Console.WriteLine($"✓ Created order for {newCustomer.Name} - {newProduct.Name}");

// ===== FINAL SUMMARY =====
Console.WriteLine("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
Console.WriteLine("SUMMARY");
Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

Console.WriteLine($"Total Products: {await db.Products.CountAsync()} (3 from package + {await db.Products.CountAsync() - 3} added)");
Console.WriteLine($"Total Customers: {await db.Customers.CountAsync()} (2 seeded + {await db.Customers.CountAsync() - 2} added)");
Console.WriteLine($"Total Orders: {await db.Orders.CountAsync()} (2 seeded + {await db.Orders.CountAsync() - 2} added)");
Console.WriteLine("\n✓ Demo complete! Both MainApp and MyPackage models working together.");
