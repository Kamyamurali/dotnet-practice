namespace CoffeeShop.Models;

public class Customer
{
    public int CustomerId { get; set; }              // Primary key
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }               // nullable = optional column
    public string? Phone { get; set; }

    // One Customer has many Orders.
    public List<Order> Orders { get; set; } = new();
}
