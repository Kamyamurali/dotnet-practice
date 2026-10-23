using BankingApi.DTOs;
using BankingApi.Exceptions;
using BankingApi.Models;
using BankingApi.Repositories;

namespace BankingApi.Services;

public interface IApplicationService
{
    Task<ApplicationDto> ApplyAsync(int userId, ApplyLoanRequest request);
    Task<List<ApplicationDto>> GetMyApplicationsAsync(int userId);
    Task<List<ApplicationDto>> GetAllAsync(string? status);
    Task<ApplicationDto> GetByIdAsync(int applicationId, int userId, bool isAdmin);
    Task<ApplicationDto> ReviewAsync(int applicationId, ReviewApplicationRequest request);
    Task<LoanStatementDto> GetStatementAsync(int applicationId, int userId, bool isAdmin);
}

public class ApplicationService : IApplicationService
{
    public const int MinimumAge = 18;
    public const decimal MaxIncomeShareForEmi = 0.50m;   

    private static readonly string[] ValidStatuses = { "Pending", "Approved", "Rejected", "Disbursed", "Closed" };

    private readonly IApplicationRepository _applications;
    private readonly IProductRepository _products;
    private readonly IDisbursementRepository _disbursements;
    private readonly IRepaymentRepository _repayments;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(IApplicationRepository applications, IProductRepository products,
        IDisbursementRepository disbursements, IRepaymentRepository repayments,
        ILogger<ApplicationService> logger)
    {
        _applications = applications;
        _products = products;
        _disbursements = disbursements;
        _repayments = repayments;
        _logger = logger;
    }

    public async Task<ApplicationDto> ApplyAsync(int userId, ApplyLoanRequest request)
    {
        LoanProduct product = await _products.GetByIdAsync(request.ProductId)
            ?? throw new NotFoundException("Loan product not found.");

        if (!product.IsActive)
            throw new BadRequestException("This loan product is no longer available.");
        if (request.AmountRequested < product.MinAmount || request.AmountRequested > product.MaxAmount)
            throw new BadRequestException(
                $"Amount must be between {product.MinAmount:N0} and {product.MaxAmount:N0} for {product.Name}.");
        if (request.TenureMonths > product.MaxTenureMonths)
            throw new BadRequestException($"Maximum tenure for {product.Name} is {product.MaxTenureMonths} months.");

        DateTime today = DateTime.UtcNow.Date;
        DateTime dateOfBirth = request.DateOfBirth!.Value.Date;
        if (dateOfBirth > today)
            throw new BadRequestException("Date of birth can't be in the future.");
        int age = AgeOn(dateOfBirth, today);
        if (age < MinimumAge)
            throw new BadRequestException($"You must be at least {MinimumAge} years old to apply.");
        if (age > 100)
            throw new BadRequestException("Please check your date of birth.");

        decimal income = request.MonthlyIncome!.Value;
        decimal emi = LoanCalculator.MonthlyInstallment(request.AmountRequested, product.InterestRate, request.TenureMonths);
        if (emi > income * MaxIncomeShareForEmi)
            throw new BadRequestException(
                $"The monthly payment ({emi:N2}) is more than {MaxIncomeShareForEmi:P0} of your monthly income ({income:N2}). " +
                "Try a smaller amount or a longer tenure.");

        if (await _applications.HasPendingAsync(userId, request.ProductId))
            throw new BadRequestException("You already have a pending application for this product.");

        ApplicationDto application = await _applications.ApplyAsync(userId, request, product);
        _logger.LogInformation("User {UserId} applied for product {ProductId} (application {ApplicationId})",
            userId, request.ProductId, application.ApplicationId);
        return application;
    }

    public Task<List<ApplicationDto>> GetMyApplicationsAsync(int userId) =>
        _applications.GetByUserAsync(userId);

    public Task<List<ApplicationDto>> GetAllAsync(string? status)
    {
        if (status is not null && !ValidStatuses.Contains(status))
            throw new BadRequestException($"Status must be one of: {string.Join(", ", ValidStatuses)}.");
        return _applications.GetAllAsync(status);
    }

    public async Task<ApplicationDto> GetByIdAsync(int applicationId, int userId, bool isAdmin)
    {
        ApplicationDto application = await _applications.GetByIdAsync(applicationId)
            ?? throw new NotFoundException($"Application {applicationId} not found.");

        if (!isAdmin && application.UserId != userId)
            throw new ForbiddenException("You can only view your own applications.");

        return application;
    }

    public async Task<ApplicationDto> ReviewAsync(int applicationId, ReviewApplicationRequest request)
    {
        ApplicationDto application = await _applications.GetByIdAsync(applicationId)
            ?? throw new NotFoundException($"Application {applicationId} not found.");

        if (application.Status != "Pending")
            throw new BadRequestException("Only pending applications can be approved or rejected.");

        ApplicationDto updated = await _applications.ReviewAsync(applicationId, request.Status, request.AdminRemarks);
        _logger.LogInformation("Application {ApplicationId} {Status}", applicationId, request.Status);
        return updated;
    }

    public async Task<LoanStatementDto> GetStatementAsync(int applicationId, int userId, bool isAdmin)
    {
        ApplicationDto application = await GetByIdAsync(applicationId, userId, isAdmin);
        DisbursementDto? disbursement = await _disbursements.GetByApplicationAsync(applicationId);
        List<Repayment> repayments = await _repayments.GetByApplicationAsync(applicationId);

        decimal totalPayable = repayments.Sum(r => r.AmountDue);
        decimal totalPaid = repayments.Sum(r => r.AmountPaid);

        return new LoanStatementDto
        {
            Application = application,
            Disbursement = disbursement,
            Repayments = repayments,
            TotalPayable = totalPayable,
            TotalPaid = totalPaid,
            Outstanding = totalPayable - totalPaid,
            InstallmentsPaid = repayments.Count(r => r.Status == "Paid"),
            NextDue = repayments.Where(r => r.Status != "Paid")
                                .OrderBy(r => r.InstallmentNo)
                                .FirstOrDefault(),
            Payments = await _repayments.GetReceiptsByApplicationAsync(applicationId)
        };
    }

    private static int AgeOn(DateTime dateOfBirth, DateTime today)
    {
        int age = today.Year - dateOfBirth.Year;
        if (dateOfBirth > today.AddYears(-age)) age--;
        return age;
    }
}