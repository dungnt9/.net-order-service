using Domain.Common;

namespace Domain.Entities;

public class Order : BaseEntity
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty; // Cached from ProductService
    public decimal ProductPrice { get; set; } // Cached from ProductService
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
}