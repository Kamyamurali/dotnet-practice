using BankingApi.Data;
using BankingApi.DTOs;
using BankingApi.Models;
using BankingApi.Services;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Repositories;

public interface IDisbursementRepository
{
    Task<DisbursementDto> DisburseAsync(int applicationId);
    Task<List<DisbursementDto>> GetAllAsync();
    Task<DisbursementDto?> GetByApplicationAsync(int applicationId);
}

public class DisbursementRepository : RepositoryBase, IDisbursementRepository
{
    public DisbursementRepository(BankingDbContext db) : base(db) { }

 
    public async Task<DisbursementDto> DisburseAsync(int applicationId)
    {
        await using var dbTransaction = await Db.Database.BeginTransactionAsync();

        LoanApplication application = await Db.LoanApplications.FindAsync(applicationId)
            ?? throw new InvalidOperationException("Application was not found.");

        // 1. the payout
        Db.Disbursements.Add(new Disbursement
        {
            ApplicationId = applicationId,
            Amount = application.AmountRequested,
            Reference = $"DSB-{applicationId:D6}"                    
        });

        application.Status = "Disbursed";

        decimal emi = LoanCalculator.MonthlyInstallment(
            application.AmountRequested, application.InterestRate, application.TenureMonths);
        DateTime today = DateTime.UtcNow.Date;

        for (int month = 1; month <= application.TenureMonths; month++)
        {
            Db.Repayments.Add(new Repayment
            {
                ApplicationId = applicationId,
                InstallmentNo = month,
                DueDate = today.AddMonths(month),
                AmountDue = emi,
                AmountPaid = 0,
                Status = "Due"
            });
        }

        Db.Transactions.Add(new Transaction
        {
            UserId = application.UserId,
            ApplicationId = applicationId,
            Type = "Disbursement",
            Amount = application.AmountRequested,
            Description = $"Loan amount disbursed for application #{applicationId}"
        });

        await Db.SaveChangesAsync();
        await dbTransaction.CommitAsync();       
                                                 
        return (await GetByApplicationAsync(applicationId))!;
    }

    public Task<List<DisbursementDto>> GetAllAsync() =>
        ToDto(Db.Disbursements).OrderByDescending(d => d.DisbursedAt).ToListAsync();

    public Task<DisbursementDto?> GetByApplicationAsync(int applicationId) =>
        ToDto(Db.Disbursements.Where(d => d.ApplicationId == applicationId)).FirstOrDefaultAsync();

    private static IQueryable<DisbursementDto> ToDto(IQueryable<Disbursement> query) =>
        query.AsNoTracking().Select(d => new DisbursementDto
        {
            DisbursementId = d.DisbursementId,
            ApplicationId = d.ApplicationId,
            Amount = d.Amount,
            Reference = d.Reference,
            DisbursedAt = d.DisbursedAt,
            CustomerName = d.Application.User.FullName,
            ProductName = d.Application.Product.Name
        });
}
