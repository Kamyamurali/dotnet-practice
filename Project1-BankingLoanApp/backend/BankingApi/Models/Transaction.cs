using System.Text.Json.Serialization;

namespace BankingApi.Models;

public class Transaction
{
    public int TransactionId { get; set; }
    public int UserId { get; set; }
    public int? ApplicationId { get; set; }                
    public string Type { get; set; } = string.Empty;       
    public decimal? Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore] public User User { get; set; } = null!;
    [JsonIgnore] public LoanApplication? Application { get; set; }
}
