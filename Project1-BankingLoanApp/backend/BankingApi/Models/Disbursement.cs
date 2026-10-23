namespace BankingApi.Models;


public class Disbursement
{
    public int DisbursementId { get; set; }
    public int ApplicationId { get; set; }                 
    public decimal Amount { get; set; }
    public string Reference { get; set; } = string.Empty;  
    public DateTime DisbursedAt { get; set; } = DateTime.UtcNow;

    public LoanApplication Application { get; set; } = null!;
}
