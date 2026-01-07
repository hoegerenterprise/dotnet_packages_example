using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPackage.Models;
using MyPackage.DTOs;

namespace MyPackage.Controllers;

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
        {
            return NotFound();
        }

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
        {
            return NotFound();
        }

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
        {
            return NotFound();
        }

        _context.Set<Product>().Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
