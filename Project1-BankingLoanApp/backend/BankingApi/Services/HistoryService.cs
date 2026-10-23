using BankingApi.Models;
using BankingApi.Repositories;

namespace BankingApi.Services;

public interface IHistoryService
{
    Task<List<Transaction>> GetHistoryAsync(int userId, string? type);
    Task<CustomerSummary> GetCustomerSummaryAsync(int userId);
    Task<AdminSummary> GetAdminSummaryAsync();
    Task<List<CustomerInfo>> GetCustomersAsync();
}

public class HistoryService : IHistoryService
{
    private readonly ITransactionRepository _transactions;
    private readonly IDashboardRepository _dashboard;
    private readonly IUserRepository _users;

    public HistoryService(ITransactionRepository transactions, IDashboardRepository dashboard,
        IUserRepository users)
    {
        _transactions = transactions;
        _dashboard = dashboard;
        _users = users;
    }

    public async Task<List<Transaction>> GetHistoryAsync(int userId, string? type)
    {
        List<Transaction> history = await _transactions.GetByUserAsync(userId);
        return string.IsNullOrWhiteSpace(type)
            ? history
            : history.Where(t => t.Type.Equals(type, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public Task<CustomerSummary> GetCustomerSummaryAsync(int userId) => _dashboard.GetCustomerSummaryAsync(userId);

    public Task<AdminSummary> GetAdminSummaryAsync() => _dashboard.GetAdminSummaryAsync();

    public Task<List<CustomerInfo>> GetCustomersAsync() => _users.GetAllCustomersAsync();
}
