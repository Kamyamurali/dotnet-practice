using System.Text.Json.Serialization;

namespace BankingApi.Models;

public class Repayment
{
    public int RepaymentId { get; set; }
    public int ApplicationId { get; set; }
    public int InstallmentNo { get; set; }
    public DateTime DueDate { get; set; }              
    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime? PaidAt { get; set; }
    public string Status { get; set; } = "Due";           
    public int? PaymentId { get; set; }                 

    [JsonIgnore] public LoanApplication Application { get; set; } = null!;
    [JsonIgnore] public Payment? Payment { get; set; }
}