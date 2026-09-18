namespace CoffeeShop.Models;

// One line of an order: "2 x Cappuccino @ $3.50".
// This is the join between Order and Product, but with extra data (Quantity, UnitPrice).
public class OrderItem
{
    public int OrderItemId { get; set; }             // Primary key

    public int OrderId { get; set; }                 // FK -> Order
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }               // FK -> Product
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    // Snapshot of the price at the time of ordering (so later price changes
    // don't rewrite the history of past orders).
    public decimal UnitPrice { get; set; }
}
