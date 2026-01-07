using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MainApp.Data;
using MainApp.Models;
using MainApp.DTOs;

namespace MainApp.Controllers;

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
        {
            return NotFound();
        }

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
        var product = await _context.Products.FindAsync(createDto.ProductId);

        if (customer == null || product == null)
        {
            return BadRequest("Invalid customer or product ID");
        }

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

        // Reload with includes
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
        {
            return NotFound();
        }

        if (updateDto.CustomerId.HasValue)
        {
            var customer = await _context.Customers.FindAsync(updateDto.CustomerId.Value);
            if (customer == null)
                return BadRequest("Invalid customer ID");
            order.CustomerId = updateDto.CustomerId.Value;
        }

        if (updateDto.ProductId.HasValue)
        {
            var product = await _context.Products.FindAsync(updateDto.ProductId.Value);
            if (product == null)
                return BadRequest("Invalid product ID");
            order.ProductId = updateDto.ProductId.Value;
        }

        if (updateDto.Quantity.HasValue)
        {
            order.Quantity = updateDto.Quantity.Value;
            var product = await _context.Products.FindAsync(order.ProductId);
            if (product != null)
            {
                order.TotalAmount = product.Price * order.Quantity;
            }
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
