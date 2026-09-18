# Coffee Shop — EF Core Code-First Console App (.NET 10)

A small console application that manages a coffee shop: a menu of products,
customers, and orders. Built with **Entity Framework Core, code-first** — meaning
you write C# classes and EF generates the database from them.

---

## What "code-first" means

There are two ways to use EF Core:

- **Database-first:** the database already exists; you point a tool at it and it
  generates C# classes for you.
- **Code-first (this project):** you write the C# classes (your "models"), and EF
  builds the database schema from them using **migrations**.

Code-first keeps the schema in source control, versioned alongside your code.

---

## Project structure

```
CoffeeShop/
├── CoffeeShop.csproj          # project + NuGet package references
├── Program.cs                 # console menu (the app entry point)
├── Models/                    # the C# classes that become tables
│   ├── Category.cs
│   ├── Product.cs
│   ├── Customer.cs
│   ├── Order.cs
│   └── OrderItem.cs
└── Data/
    └── CoffeeShopContext.cs   # the DbContext — the "database" in C#
```

## The data model

Five tables and how they relate:

- **Category** 1 —→ many **Product**  (a category groups products)
- **Customer** 1 —→ many **Order**    (a customer places orders)
- **Order** 1 —→ many **OrderItem**   (an order has several lines)
- **Product** 1 —→ many **OrderItem** (a product appears on many order lines)

`OrderItem` is the bridge between orders and products, and it carries extra data
(`Quantity`, `UnitPrice`) — that's why it's its own table rather than a plain
many-to-many link.

---

## How to run it (step by step)

You need the **.NET 10 SDK** installed. Check with `dotnet --version`.

### 1. Install the EF command-line tool (once per machine)
```bash
dotnet tool install --global dotnet-ef
```

### 2. Restore the NuGet packages
```bash
cd CoffeeShop
dotnet restore
```

### 3. Create the first migration
This reads your models and generates C# code describing the schema (a
`Migrations/` folder appears).
```bash
dotnet ef migrations add InitialCreate
```

### 4. Create the database
This runs the migration against SQLite, producing `coffeeshop.db` with all tables
and the seeded menu.
```bash
dotnet ef database update
```

> Note: the app also calls `db.Database.Migrate()` on startup, so step 4 is
> optional — but running it explicitly is the cleaner thing to *show* on a panel.

### 5. Run the app
```bash
dotnet run
```

You'll get a menu:
```
1. View menu (products)
2. Add a customer
3. Place an order
4. View all orders
5. Exit
```

**Demo path to show:** `1` (see seeded menu) → `2` (add yourself) → `3` (place an
order with a couple of items) → `4` (see the order with its total). That single
loop touches Create, Read, and the relationships between all five tables.

---

## If you change a model later

Say you add a `Size` column to `Product`. The cycle is always:
```bash
dotnet ef migrations add AddProductSize   # generate the change
dotnet ef database update                 # apply it
```
Each migration is a versioned step, so the database can move forward (or roll
back with `dotnet ef database update PreviousMigrationName`).

---

## Switching to SQL Server (optional, matches the class's T-SQL work)

1. In `CoffeeShop.csproj`, replace the SQLite package with:
   `Microsoft.EntityFrameworkCore.SqlServer`.
2. In `CoffeeShopContext.cs`, comment the `UseSqlite(...)` line and uncomment the
   `UseSqlServer(...)` line.
3. Delete the old `Migrations/` folder, re-run `migrations add InitialCreate` and
   `database update`. (Providers generate slightly different SQL, so migrations
   aren't shared between them.)

Everything else — the models, the Program.cs logic — stays identical. That's the
point of EF: your C# doesn't care which database is underneath.

---

## Technical talking points (for panel questions)

**Q: What is a `DbContext`?**
It's the class that represents a session with the database. Each `DbSet<T>` on it
maps to a table. It tracks the objects you load or create ("change tracking") and
turns your changes into SQL when you call `SaveChanges()`.

**Q: How does EF know the primary key?**
Convention: a property named `Id` or `<ClassName>Id` (e.g. `ProductId`) is treated
as the primary key automatically. No attribute needed.

**Q: How are the table relationships defined?**
By convention, from the navigation properties and foreign keys. `Product` has a
`CategoryId` (the FK) and a `Category` property (the navigation). EF infers the
one-to-many relationship from that pair.

**Q: What's the difference between conventions and the Fluent API?**
Conventions are EF's default assumptions (like the PK rule above). The Fluent API,
in `OnModelCreating`, overrides or adds to them — here it sets decimal precision on
money columns and seeds the starter data.

**Q: What does `Include` do?**
By default EF only loads the entity you asked for, not its related data (this is
"lazy vs eager" loading). `Include(o => o.Items)` tells EF to also load the related
rows in the same query (it becomes a SQL JOIN). `ThenInclude` goes one level deeper.

**Q: Why store `UnitPrice` on the OrderItem instead of just reading `Product.Price`?**
So order history is accurate. If the price of a cappuccino changes next week, past
orders should still show what the customer actually paid.

**Q: What actually happens on `SaveChanges()`?**
EF looks at every object it's tracking, works out which are new / modified /
deleted, generates the matching `INSERT` / `UPDATE` / `DELETE` statements, wraps
them in a transaction, and sends them to the database. It also reads back
auto-generated values like the new identity Id.

**Q: What is a migration, really?**
A pair of `Up()` / `Down()` methods (auto-generated C#) describing how to move the
schema forward or back. EF also keeps a `__EFMigrationsHistory` table in the
database so it knows which migrations have already been applied.

**Q: SQLite vs SQL Server here?**
Same code, different provider. SQLite is a single file — zero setup — which is why
it's used for this demo. Swapping to SQL Server is a two-line change plus
regenerating migrations.
