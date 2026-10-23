using BankingApi.Data;
using BankingApi.DTOs;
using BankingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Repositories;

public interface IApplicationRepository
{
    Task<ApplicationDto> ApplyAsync(int userId, ApplyLoanRequest request, LoanProduct product);
    Task<bool> HasPendingAsync(int userId, int productId);
    Task<List<ApplicationDto>> GetByUserAsync(int userId);
    Task<List<ApplicationDto>> GetAllAsync(string? status);
    Task<ApplicationDto?> GetByIdAsync(int applicationId);
    Task<ApplicationDto> ReviewAsync(int applicationId, string status, string? remarks);
}

public class ApplicationRepository : RepositoryBase, IApplicationRepository
{
    public ApplicationRepository(BankingDbContext db) : base(db) { }

    public async Task<ApplicationDto> ApplyAsync(int userId, ApplyLoanRequest request, LoanProduct product)
    {
        var application = new LoanApplication
        {
            UserId = userId,
            ProductId = product.ProductId,
            AmountRequested = request.AmountRequested,
            TenureMonths = request.TenureMonths,
            InterestRate = product.InterestRate,           
            Purpose = request.Purpose.Trim(),
            ApplicantDateOfBirth = request.DateOfBirth!.Value.Date,
            MonthlyIncome = request.MonthlyIncome,
            Status = "Pending"
        };

        Db.LoanApplications.Add(application);
        Db.Transactions.Add(new Transaction
        {
            UserId = userId,
            Application = application,                     
            Type = "Application",
            Amount = request.AmountRequested,
            Description = $"Applied for {product.Name}"
        });

        await Db.SaveChangesAsync();                    
        return (await GetByIdAsync(application.ApplicationId))!;
    }

    public Task<bool> HasPendingAsync(int userId, int productId) =>
        Db.LoanApplications.AnyAsync(a => a.UserId == userId && a.ProductId == productId && a.Status == "Pending");

    public Task<List<ApplicationDto>> GetByUserAsync(int userId) =>
        ToDto(Db.LoanApplications.Where(a => a.UserId == userId))
          .OrderByDescending(a => a.AppliedAt)
          .ToListAsync();

    public Task<List<ApplicationDto>> GetAllAsync(string? status) =>
        ToDto(Db.LoanApplications.Where(a => status == null || a.Status == status))
          .OrderByDescending(a => a.AppliedAt)
          .ToListAsync();

    public Task<ApplicationDto?> GetByIdAsync(int applicationId) =>
        ToDto(Db.LoanApplications.Where(a => a.ApplicationId == applicationId))
          .FirstOrDefaultAsync();

    public async Task<ApplicationDto> ReviewAsync(int applicationId, string status, string? remarks)
    {
        LoanApplication application = await Db.LoanApplications.FindAsync(applicationId)
            ?? throw new InvalidOperationException("Application was not found.");

        application.Status = status;
        application.AdminRemarks = remarks;
        application.ReviewedAt = DateTime.UtcNow;

        Db.Transactions.Add(new Transaction
        {
            UserId = application.UserId,
            ApplicationId = applicationId,
            Type = status == "Approved" ? "Approval" : "Rejection",
            Amount = application.AmountRequested,
            Description = $"Application #{applicationId} {status.ToLowerInvariant()}"
        });

        await Db.SaveChangesAsync();                       
        return (await GetByIdAsync(applicationId))!;
    }

  
    private static IQueryable<ApplicationDto> ToDto(IQueryable<LoanApplication> query) =>
        query.AsNoTracking().Select(a => new ApplicationDto
        {
            ApplicationId = a.ApplicationId,
            UserId = a.UserId,
            CustomerName = a.User.FullName,
            CustomerEmail = a.User.Email,
            ProductId = a.ProductId,
            ProductName = a.Product.Name,
            Category = a.Product.Category,
            AmountRequested = a.AmountRequested,
            TenureMonths = a.TenureMonths,
            InterestRate = a.InterestRate,
            Purpose = a.Purpose,
            ApplicantDateOfBirth = a.ApplicantDateOfBirth,
            MonthlyIncome = a.MonthlyIncome,
            Status = a.Status,
            AdminRemarks = a.AdminRemarks,
            AppliedAt = a.AppliedAt,
            ReviewedAt = a.ReviewedAt
        });
}