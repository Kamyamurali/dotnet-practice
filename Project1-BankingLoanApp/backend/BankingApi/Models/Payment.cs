using System.Text.Json.Serialization;

namespace BankingApi.Models;

public class Payment
{
    public int PaymentId { get; set; }
    public string TransactionRef { get; set; } = string.Empty;   
    public int ApplicationId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = "UPI";                  
    public string Type { get; set; } = "Installment";            
    public string Status { get; set; } = "Success";            
    public string Description { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore] public LoanApplication Application { get; set; } = null!;
    [JsonIgnore] public User User { get; set; } = null!;
    [JsonIgnore] public List<Repayment> Repayments { get; set; } = new();
}