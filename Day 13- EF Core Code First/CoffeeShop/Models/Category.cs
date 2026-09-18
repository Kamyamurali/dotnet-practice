namespace CoffeeShop.Models;

// A grouping for products, e.g. "Hot Coffee", "Cold Coffee", "Pastries".
public class Category
{
    public int CategoryId { get; set; }              // Primary key (EF picks up the "<Class>Id" convention)
    public string Name { get; set; } = string.Empty;

    // Navigation property: one Category has many Products.
    public List<Product> Products { get; set; } = new();
}
