using BankingApi.Data;
using BankingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Repositories;

public interface ITransactionRepository
{
    Task<List<Transaction>> GetByUserAsync(int userId);
}

public class TransactionRepository : RepositoryBase, ITransactionRepository
{
    public TransactionRepository(BankingDbContext db) : base(db) { }

    public Task<List<Transaction>> GetByUserAsync(int userId) =>
        Db.Transactions.AsNoTracking()
          .Where(t => t.UserId == userId)
          .OrderByDescending(t => t.CreatedAt)
          .ThenByDescending(t => t.TransactionId)
          .ToListAsync();
}
