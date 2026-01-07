using MainApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Users.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddApplicationPart(typeof(MyPackage.Controllers.ProductsController).Assembly) // Discover controllers from MyPackage
    .AddApplicationPart(typeof(Users.Controllers.AuthController).Assembly); // Discover controllers from Users

// Add DbContext
builder.Services.AddDbContext<AppDbContext>();
// Register DbContext so MyPackage controllers can use it
builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<AppDbContext>());

// Add JWT Service
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? "your-super-secret-key-min-32-chars-long";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "dotnet-packages-demo";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "dotnet-packages-demo-api";
var jwtExpirationMinutes = int.Parse(builder.Configuration["Jwt:ExpirationMinutes"] ?? "60");

builder.Services.AddSingleton(new JwtService(jwtSecretKey, jwtIssuer, jwtAudience, jwtExpirationMinutes));

// Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
    };
});

builder.Services.AddAuthorization();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() {
        Title = ".NET 8 Package Demo API",
        Version = "v1",
        Description = "API demonstrating reusable packages with Entity Framework Core. Includes controllers from both MainApp, MyPackage, and Users package with JWT authentication."
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Logger.LogInformation("API started successfully!");
app.Logger.LogInformation("Swagger UI: http://localhost:5000/docs");
app.Logger.LogInformation("API Base: http://localhost:5000/api/v1");
app.Logger.LogInformation("Controllers:");
app.Logger.LogInformation("  - CustomersController (MainApp)");
app.Logger.LogInformation("  - OrdersController (MainApp)");
app.Logger.LogInformation("  - ProductsController (MyPackage)");
app.Logger.LogInformation("  - AuthController (Users)");
app.Logger.LogInformation("  - UsersController (Users)");
app.Logger.LogInformation("  - UserGroupsController (Users)");

app.Run();
