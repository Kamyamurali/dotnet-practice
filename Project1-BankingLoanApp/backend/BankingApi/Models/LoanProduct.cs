using System.Text.Json.Serialization;

namespace BankingApi.Models;

public class LoanProduct
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal InterestRate { get; set; }           
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public int MaxTenureMonths { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore] public List<LoanApplication> Applications { get; set; } = new();
}
