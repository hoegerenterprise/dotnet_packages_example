using MainApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddApplicationPart(typeof(MyPackage.Controllers.ProductsController).Assembly); // Discover controllers from MyPackage

// Add DbContext
builder.Services.AddDbContext<AppDbContext>();
// Register DbContext so MyPackage controllers can use it
builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<AppDbContext>());

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() {
        Title = ".NET 8 Package Demo API",
        Version = "v1",
        Description = "API demonstrating reusable packages with Entity Framework Core. Includes controllers from both MainApp and MyPackage."
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
// Enable Swagger in all environments for demo purposes
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", ".NET 8 Package Demo API v1");
    c.RoutePrefix = "docs"; // Serve Swagger UI at /docs
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Logger.LogInformation("API started successfully!");
app.Logger.LogInformation("Swagger UI: http://localhost:5000/docs");
app.Logger.LogInformation("API Base: http://localhost:5000/api/v1");
app.Logger.LogInformation("Controllers:");
app.Logger.LogInformation("  - CustomersController (MainApp)");
app.Logger.LogInformation("  - OrdersController (MainApp)");
app.Logger.LogInformation("  - ProductsController (MyPackage)");

app.Run();
