using Microsoft.EntityFrameworkCore;

namespace BankingApi.Data;
public abstract class RepositoryBase
{
    protected readonly BankingDbContext Db;

    protected RepositoryBase(BankingDbContext db)
    {
        Db = db;
    }

 
    protected Task<int> RefreshOverdueAsync()
    {
        DateTime today = DateTime.UtcNow.Date;
        return Db.Repayments
                 .Where(r => r.Status == "Due" && r.DueDate < today)
                 .ExecuteUpdateAsync(s => s.SetProperty(r => r.Status, "Overdue"));
    }
}
