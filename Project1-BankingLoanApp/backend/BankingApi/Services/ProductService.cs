using BankingApi.DTOs;
using BankingApi.Exceptions;
using BankingApi.Models;
using BankingApi.Repositories;

namespace BankingApi.Services;

public interface IProductService
{
    Task<List<LoanProduct>> GetAllAsync(bool includeInactive, string? category);
    Task<LoanProduct> GetByIdAsync(int productId);
    Task<LoanProduct> AddAsync(ProductRequest request);
    Task<LoanProduct> UpdateAsync(int productId, ProductRequest request);
    Task<string> DeleteAsync(int productId);
}

public class ProductService : IProductService
{
    private readonly IProductRepository _products;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository products, ILogger<ProductService> logger)
    {
        _products = products;
        _logger = logger;
    }

    public async Task<List<LoanProduct>> GetAllAsync(bool includeInactive, string? category)
    {
        List<LoanProduct> products = await _products.GetAllAsync(includeInactive);

        // Optional filter using LINQ
        if (!string.IsNullOrWhiteSpace(category))
            products = products
                .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToList();

        return products;
    }

    public async Task<LoanProduct> GetByIdAsync(int productId) =>
        await _products.GetByIdAsync(productId)
        ?? throw new NotFoundException($"Loan product {productId} not found.");

    public async Task<LoanProduct> AddAsync(ProductRequest request)
    {
        ValidateRange(request);
        LoanProduct product = await _products.AddAsync(request);
        _logger.LogInformation("Product {ProductId} '{Name}' added", product.ProductId, product.Name);
        return product;
    }

    public async Task<LoanProduct> UpdateAsync(int productId, ProductRequest request)
    {
        ValidateRange(request);
        await GetByIdAsync(productId);            // 404 if it doesn't exist
        return await _products.UpdateAsync(productId, request);
    }

    public async Task<string> DeleteAsync(int productId)
    {
        await GetByIdAsync(productId);
        string result = await _products.DeleteAsync(productId);
        _logger.LogInformation("Product {ProductId} {Result}", productId, result);
        return result;
    }

    private static void ValidateRange(ProductRequest request)
    {
        if (request.MaxAmount < request.MinAmount)
            throw new BadRequestException("Maximum amount must be greater than or equal to minimum amount.");
    }
}
