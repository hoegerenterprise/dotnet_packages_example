namespace MainApp.DTOs;

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
