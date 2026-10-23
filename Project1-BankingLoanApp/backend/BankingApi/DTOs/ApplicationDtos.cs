using System.ComponentModel.DataAnnotations;
using BankingApi.Models;

namespace BankingApi.DTOs;

public class ApplyLoanRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Please choose a loan product.")]
    public int ProductId { get; set; }

    [Range(1, 100_000_000, ErrorMessage = "Amount must be greater than 0.")]
    public decimal AmountRequested { get; set; }

    [Range(1, 480, ErrorMessage = "Tenure must be between 1 and 480 months.")]
    public int TenureMonths { get; set; }

    [Required, StringLength(300, MinimumLength = 3)]
    public string Purpose { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your date of birth.")]
    public DateTime? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Please enter your monthly income.")]
    [Range(1, 100_000_000, ErrorMessage = "Monthly income must be greater than 0.")]
    public decimal? MonthlyIncome { get; set; }
}

public class ReviewApplicationRequest
{
    [Required]
    [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Status must be Approved or Rejected.")]
    public string Status { get; set; } = string.Empty;

    [StringLength(300)]
    public string? AdminRemarks { get; set; }
}

public class UpdateRepaymentStatusRequest
{
    [Required]
    [RegularExpression("^(Paid|Due|Overdue)$", ErrorMessage = "Status must be Paid, Due or Overdue.")]
    public string Status { get; set; } = string.Empty;
}

public class ApplicationDto
{
    public int ApplicationId { get; set; }
    public int UserId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal AmountRequested { get; set; }
    public int TenureMonths { get; set; }
    public decimal InterestRate { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public DateTime? ApplicantDateOfBirth { get; set; }
    public decimal? MonthlyIncome { get; set; }
    public string Status { get; set; } = "Pending";
    public string? AdminRemarks { get; set; }
    public DateTime AppliedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
}

public class DisbursementDto
{
    public int DisbursementId { get; set; }
    public int ApplicationId { get; set; }
    public decimal Amount { get; set; }
    public string Reference { get; set; } = string.Empty;
    public DateTime DisbursedAt { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
}

public class PayInstallmentRequest
{
    [Required]
    [RegularExpression("^(Card|UPI|NetBanking)$", ErrorMessage = "Payment method must be Card, UPI or NetBanking.")]
    public string PaymentMethod { get; set; } = string.Empty;
}

public class PaymentReceiptDto
{
    public int PaymentId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public int ApplicationId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;        
    public string Status { get; set; } = string.Empty;        
    public string Description { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
    public List<int> InstallmentNos { get; set; } = new();    
}

public class PayoffQuoteDto
{
    public int ApplicationId { get; set; }
    public int InstallmentsRemaining { get; set; }
    public decimal ScheduledRemaining { get; set; }   
    public decimal PayoffAmount { get; set; }        
    public decimal InterestSaved { get; set; }
}

public class LoanStatementDto
{
    public ApplicationDto Application { get; set; } = new();
    public DisbursementDto? Disbursement { get; set; }
    public List<Repayment> Repayments { get; set; } = new();

    public decimal TotalPayable { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Outstanding { get; set; }
    public int InstallmentsPaid { get; set; }
    public Repayment? NextDue { get; set; }
    public List<PaymentReceiptDto> Payments { get; set; } = new();
}