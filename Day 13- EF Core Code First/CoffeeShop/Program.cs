using CoffeeShop.Data;
using CoffeeShop.Models;
using Microsoft.EntityFrameworkCore;

// One DbContext for the app session. "using" disposes it (closes the connection) at the end.
using var db = new CoffeeShopContext();

// Apply any pending migrations at startup. On first run this CREATES the database
// and all tables from your migration, then inserts the seed data.
db.Database.Migrate();

Console.WriteLine("=== Welcome to the Coffee Shop ===\n");

bool running = true;
while (running)
{
    Console.WriteLine("""
        1. View menu (products)
        2. Add a customer
        3. Place an order
        4. View all orders
        5. Exit
        """);
    Console.Write("Choose an option: ");
    var choice = Console.ReadLine();
    Console.WriteLine();

    switch (choice)
    {
        case "1": ViewProducts(db); break;
        case "2": AddCustomer(db);  break;
        case "3": PlaceOrder(db);   break;
        case "4": ViewOrders(db);   break;
        case "5": running = false;  break;
        default:  Console.WriteLine("Invalid option.\n"); break;
    }
}

Console.WriteLine("Goodbye!");


// ---- READ: list products, pulling in each product's Category via Include ----
static void ViewProducts(CoffeeShopContext db)
{
    var products = db.Products
        .Include(p => p.Category)          // eager-load the related Category (a JOIN)
        .OrderBy(p => p.Category.Name)
        .ToList();

    Console.WriteLine("--- Menu ---");
    foreach (var p in products)
        Console.WriteLine($"[{p.ProductId}] {p.Name} ({p.Category.Name}) - ${p.Price:0.00}");
    Console.WriteLine();
}

// ---- CREATE: insert a new Customer ----
static void AddCustomer(CoffeeShopContext db)
{
    Console.Write("Customer name: ");
    var name = Console.ReadLine() ?? "";
    Console.Write("Email (optional): ");
    var email = Console.ReadLine();

    var customer = new Customer { Name = name, Email = string.IsNullOrWhiteSpace(email) ? null : email };

    db.Customers.Add(customer);   // stage the insert
    db.SaveChanges();             // EF generates & runs the INSERT, fills in the new Id

    Console.WriteLine($"Added customer #{customer.CustomerId} - {customer.Name}\n");
}

// ---- CREATE (parent + children in one save): an Order with its OrderItems ----
static void PlaceOrder(CoffeeShopContext db)
{
    var customers = db.Customers.ToList();
    if (customers.Count == 0)
    {
        Console.WriteLine("No customers yet. Add one first (option 2).\n");
        return;
    }

    Console.WriteLine("Customers:");
    foreach (var c in customers)
        Console.WriteLine($"[{c.CustomerId}] {c.Name}");
    Console.Write("Choose customer id: ");
    if (!int.TryParse(Console.ReadLine(), out int customerId) ||
        !customers.Any(c => c.CustomerId == customerId))
    {
        Console.WriteLine("Invalid customer.\n");
        return;
    }

    var order = new Order { CustomerId = customerId };

    while (true)
    {
        ViewProducts(db);
        Console.Write("Enter product id to add (0 to finish): ");
        if (!int.TryParse(Console.ReadLine(), out int productId) || productId == 0)
            break;

        var product = db.Products.Find(productId);   // Find = lookup by primary key
        if (product is null)
        {
            Console.WriteLine("No such product.\n");
            continue;
        }

        Console.Write("Quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0) qty = 1;

        order.Items.Add(new OrderItem
        {
            ProductId = product.ProductId,
            Quantity  = qty,
            UnitPrice = product.Price     // capture the price at order time
        });

        Console.WriteLine($"Added {qty} x {product.Name}\n");
    }

    if (order.Items.Count == 0)
    {
        Console.WriteLine("Order cancelled - no items.\n");
        return;
    }

    // Adding the Order also inserts its Items, because EF tracks the whole graph.
    db.Orders.Add(order);
    db.SaveChanges();

    Console.WriteLine($"Order #{order.OrderId} placed. Total: ${order.Total:0.00}\n");
}

// ---- READ (nested): orders with customer + items + each item's product ----
static void ViewOrders(CoffeeShopContext db)
{
    var orders = db.Orders
        .Include(o => o.Customer)
        .Include(o => o.Items)
            .ThenInclude(i => i.Product)   // go a level deeper: Items -> Product
        .OrderByDescending(o => o.OrderDate)
        .ToList();

    if (orders.Count == 0)
    {
        Console.WriteLine("No orders yet.\n");
        return;
    }

    foreach (var o in orders)
    {
        Console.WriteLine($"Order #{o.OrderId} | {o.Customer.Name} | {o.OrderDate:g}");
        foreach (var item in o.Items)
            Console.WriteLine($"   {item.Quantity} x {item.Product.Name} @ ${item.UnitPrice:0.00}");
        Console.WriteLine($"   Total: ${o.Total:0.00}\n");
    }
}
