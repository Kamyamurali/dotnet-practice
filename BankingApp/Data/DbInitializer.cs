
using BankingApp.Models;
using BankingApp.Security;

namespace BankingApp.Data;

public static class DbInitializer
{
    public static void Seed(BankingDbContext db)
    {
        if (db.Customers.Any()) return; 

        db.Admins.Add(new Admin { Username = "admin", Password = PasswordHasher.Hash("admin@123") });

        var kamya = new Customer
        {
            Username = "customer",
            Password = PasswordHasher.Hash("cust@123"),
            FullName = "Kamya Murali",
            Email = "customer@email.com",
            Phone = "1234567890"
        };
        kamya.Accounts.Add(new Account { AccountNumber = "AC1001", AccountType = "Savings", Balance = 25000 });
        kamya.Accounts.Add(new Account { AccountNumber = "AC1002", AccountType = "Current", Balance = 12000 });

        var priya = new Customer
        {
            Username = "priya",
            Password = PasswordHasher.Hash("priya@123"),
            FullName = "Priya Nair",
            Email = "priya@email.com",
            Phone = "9876543210"
        };
        priya.Accounts.Add(new Account { AccountNumber = "AC1003", AccountType = "Current", Balance = 50000 });

        db.Customers.AddRange(kamya, priya);
        db.SaveChanges();

        var ac1001 = db.Accounts.First(a => a.AccountNumber == "AC1001");
        db.Transactions.Add(new Transaction { AccountId = ac1001.AccountId, Type = "Deposit", Amount = 10000, BalanceAfter = 25000, TransactionDate = DateTime.Now.AddDays(-5) });
        db.Transactions.Add(new Transaction { AccountId = ac1001.AccountId, Type = "Withdrawal", Amount = 5000, BalanceAfter = 25000, TransactionDate = DateTime.Now.AddDays(-2) });
        db.SaveChanges();
    }
}