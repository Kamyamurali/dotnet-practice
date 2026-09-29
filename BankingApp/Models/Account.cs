namespace BankingApp.Models;

public class Account
{
    public int AccountId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountType { get; set; } = "Savings";   
    public decimal Balance { get; set; }
    public bool IsActive { get; set; } = true;

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public List<Transaction> Transactions { get; set; } = new();
}