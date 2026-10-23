namespace BankingApi.Models;

public class LoanApplication
{
    public int ApplicationId { get; set; }
    public int UserId { get; set; }                       
    public int ProductId { get; set; }                    
    public decimal AmountRequested { get; set; }
    public int TenureMonths { get; set; }
    public decimal InterestRate { get; set; }              
    public string Purpose { get; set; } = string.Empty;


    public DateTime? ApplicantDateOfBirth { get; set; }      
    public decimal? MonthlyIncome { get; set; }
    public string Status { get; set; } = "Pending";       
    public string? AdminRemarks { get; set; }
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }

    public User User { get; set; } = null!;
    public LoanProduct Product { get; set; } = null!;
    public Disbursement? Disbursement { get; set; }       
    public List<Repayment> Repayments { get; set; } = new();
}