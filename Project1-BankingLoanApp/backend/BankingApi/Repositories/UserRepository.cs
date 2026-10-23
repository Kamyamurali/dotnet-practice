using BankingApi.Data;
using BankingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Repositories;

public interface IUserRepository
{
    Task<User> RegisterAsync(string fullName, string email, string? phone, string passwordHash, string role = "Customer");
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int userId);
    Task<bool> AnyUsersAsync();
    Task<List<CustomerInfo>> GetAllCustomersAsync();
}

public class UserRepository : RepositoryBase, IUserRepository
{
    public UserRepository(BankingDbContext db) : base(db) { }

    public async Task<User> RegisterAsync(string fullName, string email, string? phone,
                                          string passwordHash, string role = "Customer")
    {
        var user = new User
        {
            FullName = fullName,
            Email = email,
            Phone = phone,
            PasswordHash = passwordHash,
            Role = role
        };

       
        user.Transactions.Add(new Transaction { Type = "Registration", Description = "Registered as a member" });

        Db.Users.Add(user);
        await Db.SaveChangesAsync();         
        return user;
    }

    public Task<User?> GetByEmailAsync(string email) =>
        Db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

    public Task<User?> GetByIdAsync(int userId) =>
        Db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);

    public Task<bool> AnyUsersAsync() => Db.Users.AnyAsync();

    public Task<List<CustomerInfo>> GetAllCustomersAsync() =>
        Db.Users.AsNoTracking()
          .Where(u => u.Role == "Customer")
          .OrderByDescending(u => u.CreatedAt)
          .Select(u => new CustomerInfo
          {
              UserId = u.UserId,
              FullName = u.FullName,
              Email = u.Email,
              Phone = u.Phone,
              CreatedAt = u.CreatedAt,
              TotalApplications = u.Applications.Count()
          })
          .ToListAsync();
}
