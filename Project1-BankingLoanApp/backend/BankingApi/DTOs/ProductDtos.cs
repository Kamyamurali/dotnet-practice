using System.ComponentModel.DataAnnotations;

namespace BankingApi.DTOs;

public class ProductRequest
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0, 50, ErrorMessage = "Interest rate must be between 0 and 50.")]
    public decimal InterestRate { get; set; }

    [Range(1, 100_000_000, ErrorMessage = "Minimum amount must be greater than 0.")]
    public decimal MinAmount { get; set; }

    [Range(1, 100_000_000, ErrorMessage = "Maximum amount must be greater than 0.")]
    public decimal MaxAmount { get; set; }

    [Range(1, 480, ErrorMessage = "Tenure must be between 1 and 480 months.")]
    public int MaxTenureMonths { get; set; }

    public bool IsActive { get; set; } = true;
}
