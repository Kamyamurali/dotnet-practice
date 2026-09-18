namespace CoffeeShop.Models;

// A single item on the menu.
public class Product
{
    public int ProductId { get; set; }               // Primary key
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    // Foreign key + navigation to the owning Category (many Products -> one Category).
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    // A product can appear on many order lines.
    public List<OrderItem> OrderItems { get; set; } = new();
}
