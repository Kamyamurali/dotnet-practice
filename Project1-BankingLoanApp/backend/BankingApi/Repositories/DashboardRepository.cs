using BankingApi.Data;
using BankingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Repositories;

public interface IDashboardRepository
{
    Task<CustomerSummary> GetCustomerSummaryAsync(int userId);
    Task<AdminSummary> GetAdminSummaryAsync();
}

public class DashboardRepository : RepositoryBase, IDashboardRepository
{
    public DashboardRepository(BankingDbContext db) : base(db) { }

    public async Task<CustomerSummary> GetCustomerSummaryAsync(int userId)
    {
        await RefreshOverdueAsync();

        var myApplications = Db.LoanApplications.Where(a => a.UserId == userId);
        var myRepayments = Db.Repayments.Where(r => r.Application.UserId == userId);

        return new CustomerSummary
        {
            TotalApplications = await myApplications.CountAsync(),
            PendingApplications = await myApplications.CountAsync(a => a.Status == "Pending"),
            ActiveLoans = await myApplications.CountAsync(a => a.Status == "Disbursed"),
            OutstandingAmount = await myRepayments.Where(r => r.Status != "Paid")
                                                  .SumAsync(r => r.AmountDue - r.AmountPaid),
            OverdueInstallments = await myRepayments.CountAsync(r => r.Status == "Overdue")
        };
    }

    public async Task<AdminSummary> GetAdminSummaryAsync()
    {
        await RefreshOverdueAsync();

        return new AdminSummary
        {
            TotalCustomers = await Db.Users.CountAsync(u => u.Role == "Customer"),
            ActiveProducts = await Db.LoanProducts.CountAsync(p => p.IsActive),
            PendingApplications = await Db.LoanApplications.CountAsync(a => a.Status == "Pending"),
            AwaitingDisbursement = await Db.LoanApplications.CountAsync(a => a.Status == "Approved"),
            TotalDisbursed = await Db.Disbursements.SumAsync(d => d.Amount),
            TotalRepaid = await Db.Repayments.SumAsync(r => r.AmountPaid),
            OverdueInstallments = await Db.Repayments.CountAsync(r => r.Status == "Overdue")
        };
    }
}
