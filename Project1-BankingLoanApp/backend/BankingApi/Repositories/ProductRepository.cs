using BankingApi.Data;
using BankingApi.DTOs;
using BankingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Repositories;

public interface IProductRepository
{
    Task<List<LoanProduct>> GetAllAsync(bool includeInactive);
    Task<LoanProduct?> GetByIdAsync(int productId);
    Task<LoanProduct> AddAsync(ProductRequest request);
    Task<LoanProduct> UpdateAsync(int productId, ProductRequest request);
    Task<string> DeleteAsync(int productId);
}

public class ProductRepository : RepositoryBase, IProductRepository
{
    public ProductRepository(BankingDbContext db) : base(db) { }

    public Task<List<LoanProduct>> GetAllAsync(bool includeInactive) =>
        Db.LoanProducts
          .AsNoTracking()                                  
          .Where(p => includeInactive || p.IsActive)        
          .OrderBy(p => p.Category).ThenBy(p => p.Name)    
          .ToListAsync();                                 

    public Task<LoanProduct?> GetByIdAsync(int productId) =>
        Db.LoanProducts.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == productId);

    public async Task<LoanProduct> AddAsync(ProductRequest r)
    {
        var product = new LoanProduct
        {
            Name = r.Name,
            Category = r.Category,
            Description = r.Description,
            InterestRate = r.InterestRate,
            MinAmount = r.MinAmount,
            MaxAmount = r.MaxAmount,
            MaxTenureMonths = r.MaxTenureMonths,
            IsActive = true
        };

        Db.LoanProducts.Add(product);
        await Db.SaveChangesAsync();                         
        return product;
    }

    public async Task<LoanProduct> UpdateAsync(int productId, ProductRequest r)
    {
        LoanProduct product = await Db.LoanProducts.FindAsync(productId)
            ?? throw new InvalidOperationException("Product was not found.");

        product.Name = r.Name;
        product.Category = r.Category;
        product.Description = r.Description;
        product.InterestRate = r.InterestRate;
        product.MinAmount = r.MinAmount;
        product.MaxAmount = r.MaxAmount;
        product.MaxTenureMonths = r.MaxTenureMonths;
        product.IsActive = r.IsActive;

        await Db.SaveChangesAsync();                         
        return product;
    }

    public async Task<string> DeleteAsync(int productId)
    {
        LoanProduct product = await Db.LoanProducts.FindAsync(productId)
            ?? throw new InvalidOperationException("Product was not found.");

        bool hasApplications = await Db.LoanApplications.AnyAsync(a => a.ProductId == productId);

        if (hasApplications)
        {
            product.IsActive = false;
            await Db.SaveChangesAsync();
            return "Deactivated";
        }

        Db.LoanProducts.Remove(product);
        await Db.SaveChangesAsync();                       
        return "Deleted";
    }
}
