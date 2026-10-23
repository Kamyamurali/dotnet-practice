namespace BankingApi.Models;

public class CustomerSummary
{
    public int TotalApplications { get; set; }
    public int PendingApplications { get; set; }
    public int ActiveLoans { get; set; }
    public decimal OutstandingAmount { get; set; }
    public int OverdueInstallments { get; set; }
}

public class AdminSummary
{
    public int TotalCustomers { get; set; }
    public int ActiveProducts { get; set; }
    public int PendingApplications { get; set; }
    public int AwaitingDisbursement { get; set; }
    public decimal TotalDisbursed { get; set; }
    public decimal TotalRepaid { get; set; }
    public int OverdueInstallments { get; set; }
}

public class CustomerInfo
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalApplications { get; set; }
}
