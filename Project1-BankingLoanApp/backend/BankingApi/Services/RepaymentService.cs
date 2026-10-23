using BankingApi.DTOs;
using BankingApi.Exceptions;
using BankingApi.Models;
using BankingApi.Repositories;

namespace BankingApi.Services;

public interface IRepaymentService
{
    Task<List<Repayment>> GetByApplicationAsync(int applicationId, int userId, bool isAdmin);
    Task<Repayment> UpdateStatusAsync(int repaymentId, string status);
    Task<PaymentReceiptDto> PayAsync(int repaymentId, int userId, string paymentMethod);
    Task<PayoffQuoteDto> GetPayoffQuoteAsync(int applicationId, int userId, bool isAdmin);
    Task<PaymentReceiptDto> PayOffAsync(int applicationId, int userId, string paymentMethod);
    Task<PaymentReceiptDto> GetReceiptAsync(int paymentId, int userId, bool isAdmin);
}

public class RepaymentService : IRepaymentService
{
    private readonly IRepaymentRepository _repayments;
    private readonly IApplicationService _applications;
    private readonly ILogger<RepaymentService> _logger;

    public RepaymentService(IRepaymentRepository repayments, IApplicationService applications,
        ILogger<RepaymentService> logger)
    {
        _repayments = repayments;
        _applications = applications;
        _logger = logger;
    }

    public async Task<List<Repayment>> GetByApplicationAsync(int applicationId, int userId, bool isAdmin)
    {
        await _applications.GetByIdAsync(applicationId, userId, isAdmin);
        return await _repayments.GetByApplicationAsync(applicationId);
    }

    public async Task<Repayment> UpdateStatusAsync(int repaymentId, string status)
    {
        Repayment repayment = await _repayments.GetByIdAsync(repaymentId)
            ?? throw new NotFoundException("Repayment installment not found.");

        if (repayment.Application.Status is not ("Disbursed" or "Closed"))
            throw new BadRequestException("This loan has not been disbursed.");
        if (repayment.Status == status)
            throw new BadRequestException("Installment already has this status.");

        repayment = await _repayments.UpdateStatusAsync(repayment, status);
        _logger.LogInformation("Repayment {RepaymentId} set to {Status}", repaymentId, status);
        return repayment;
    }

    public async Task<PaymentReceiptDto> PayAsync(int repaymentId, int userId, string paymentMethod)
    {
        Repayment repayment = await _repayments.GetByIdAsync(repaymentId)
            ?? throw new NotFoundException("Repayment installment not found.");

        if (repayment.Application.UserId != userId)
            throw new ForbiddenException("You can only pay installments on your own loans.");
        if (repayment.Application.Status != "Disbursed")
            throw new BadRequestException("This loan is not active.");
        if (repayment.Status == "Paid")
            throw new BadRequestException("This installment is already paid.");

        Repayment? firstUnpaid = await _repayments.GetFirstUnpaidAsync(repayment.ApplicationId);
        if (firstUnpaid is not null && firstUnpaid.RepaymentId != repaymentId)
            throw new BadRequestException($"Please pay installment {firstUnpaid.InstallmentNo} first.");

        repayment = await _repayments.UpdateStatusAsync(repayment, "Paid", paidVia: paymentMethod);
        _logger.LogInformation("User {UserId} paid installment {RepaymentId} online via {Method}",
            userId, repaymentId, paymentMethod);

        return (await _repayments.GetReceiptAsync(repayment.PaymentId!.Value))!;
    }

    public async Task<PayoffQuoteDto> GetPayoffQuoteAsync(int applicationId, int userId, bool isAdmin)
    {
        ApplicationDto application = await _applications.GetByIdAsync(applicationId, userId, isAdmin); // 404 / 403
        if (application.Status != "Disbursed")
            throw new BadRequestException("Only active (disbursed) loans can be repaid early.");

        List<Repayment> schedule = await _repayments.GetByApplicationAsync(applicationId);
        List<Repayment> unpaid = schedule.Where(r => r.Status != "Paid").ToList();
        if (unpaid.Count == 0)
            throw new BadRequestException("This loan is already fully paid.");

        int paidCount = schedule.Count - unpaid.Count;
        decimal scheduledRemaining = unpaid.Sum(r => r.AmountDue);
        decimal payoff = LoanCalculator.RemainingPrincipal(
            application.AmountRequested, application.InterestRate, application.TenureMonths, paidCount);
        payoff = Math.Min(payoff, scheduledRemaining);     

        return new PayoffQuoteDto
        {
            ApplicationId = applicationId,
            InstallmentsRemaining = unpaid.Count,
            ScheduledRemaining = scheduledRemaining,
            PayoffAmount = payoff,
            InterestSaved = scheduledRemaining - payoff
        };
    }

    public async Task<PaymentReceiptDto> PayOffAsync(int applicationId, int userId, string paymentMethod)
    {
        PayoffQuoteDto quote = await GetPayoffQuoteAsync(applicationId, userId, isAdmin: false);
        if (quote.PayoffAmount <= 0)
            throw new BadRequestException("Nothing left to pay on this loan.");

        PaymentReceiptDto receipt = await _repayments.PayOffAsync(applicationId, quote.PayoffAmount, paymentMethod);
        _logger.LogInformation("User {UserId} repaid application {ApplicationId} early: {Amount} via {Method}",
            userId, applicationId, quote.PayoffAmount, paymentMethod);
        return receipt;
    }

    public async Task<PaymentReceiptDto> GetReceiptAsync(int paymentId, int userId, bool isAdmin)
    {
        PaymentReceiptDto receipt = await _repayments.GetReceiptAsync(paymentId)
            ?? throw new NotFoundException("Receipt not found.");
        await _applications.GetByIdAsync(receipt.ApplicationId, userId, isAdmin);  
        return receipt;
    }
}