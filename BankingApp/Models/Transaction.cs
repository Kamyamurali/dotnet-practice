namespace BankingApp.Models;

public class Transaction
{
    public int TransactionId { get; set; }
    public string Type { get; set; } = string.Empty;   
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.Now;

    public int AccountId { get; set; }
    public Account? Account { get; set; }
}

// docker exec -it bankingapp-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'Str0ng!Passw0rd' -C -d BankingAppDB
//docker exec -it bankingapp-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'Str0ng!Passw0rd' -C -d BankingAppDB -W -Q "SELECT AccountNumber, Balance FROM Accounts; SELECT TOP 3 Type, Amount FROM Transactions ORDER BY TransactionDate DESC;"