using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShop.Models;

public class Order
{
    public int OrderId { get; set; }                 // Primary key
    public DateTime OrderDate { get; set; } = DateTime.Now;

    // Foreign key + navigation to the Customer who placed it.
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    // The lines that make up this order.
    public List<OrderItem> Items { get; set; } = new();

    // Calculated in C#, NOT stored as a column. [NotMapped] tells EF to ignore it.
    [NotMapped]
    public decimal Total => Items.Sum(i => i.Quantity * i.UnitPrice);
}
