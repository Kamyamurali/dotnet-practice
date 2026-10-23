using BankingApi.DTOs;
using BankingApi.Exceptions;
using BankingApi.Models;
using BankingApi.Repositories;

namespace BankingApi.Services;

public interface IDisbursementService
{
    Task<DisbursementDto> DisburseAsync(int applicationId);
    Task<List<DisbursementDto>> GetAllAsync();
}

public class DisbursementService : IDisbursementService
{
    private readonly IDisbursementRepository _disbursements;
    private readonly IApplicationRepository _applications;
    private readonly ILogger<DisbursementService> _logger;

    public DisbursementService(IDisbursementRepository disbursements,
        IApplicationRepository applications, ILogger<DisbursementService> logger)
    {
        _disbursements = disbursements;
        _applications = applications;
        _logger = logger;
    }

    public async Task<DisbursementDto> DisburseAsync(int applicationId)
    {
        ApplicationDto application = await _applications.GetByIdAsync(applicationId)
            ?? throw new NotFoundException($"Application {applicationId} not found.");

        if (application.Status != "Approved")
            throw new BadRequestException("Only approved applications can be disbursed.");

        DisbursementDto disbursement = await _disbursements.DisburseAsync(applicationId);
        _logger.LogInformation("Disbursed {Amount} for application {ApplicationId} ({Reference})",
            disbursement.Amount, applicationId, disbursement.Reference);
        return disbursement;
    }

    public Task<List<DisbursementDto>> GetAllAsync() => _disbursements.GetAllAsync();
}
