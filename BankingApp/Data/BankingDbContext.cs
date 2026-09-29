using BankingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Data;

public class BankingDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<ChequeBookRequest> ChequeBookRequests => Set<ChequeBookRequest>();
    public DbSet<Admin> Admins => Set<Admin>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      
        optionsBuilder.UseSqlServer(
            "Server=localhost,1434;Database=BankingAppDB;User Id=sa;" +
            "Password=Str0ng!Passw0rd;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .Property(a => a.Balance)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.BalanceAfter)
            .HasPrecision(18, 2);
    }
}