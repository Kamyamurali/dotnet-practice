using System.Text.Json.Serialization;

namespace BankingApi.Models;

public class User
{
    public int UserId { get; set; }                       
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }                   
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    [JsonIgnore] public List<LoanApplication> Applications { get; set; } = new();
    [JsonIgnore] public List<Transaction> Transactions { get; set; } = new();
}
