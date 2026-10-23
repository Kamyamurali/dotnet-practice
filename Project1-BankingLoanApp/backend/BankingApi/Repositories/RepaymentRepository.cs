using BankingApi.Data;
using BankingApi.DTOs;
using BankingApi.Models;
using BankingApi.Services;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Repositories;

public interface IRepaymentRepository
{
    Task<List<Repayment>> GetByApplicationAsync(int applicationId);
    Task<Repayment?> GetByIdAsync(int repaymentId);
    Task<Repayment?> GetFirstUnpaidAsync(int applicationId);
    Task<Repayment> UpdateStatusAsync(Repayment repayment, string status, string? paidVia = null);
    Task<PaymentReceiptDto> PayOffAsync(int applicationId, decimal payoffAmount, string method);
    Task<PaymentReceiptDto?> GetReceiptAsync(int paymentId);
    Task<List<PaymentReceiptDto>> GetReceiptsByApplicationAsync(int applicationId);
}

public class RepaymentRepository : RepositoryBase, IRepaymentRepository
{
    public RepaymentRepository(BankingDbContext db) : base(db) { }

    public async Task<List<Repayment>> GetByApplicationAsync(int applicationId)
    {
        await RefreshOverdueAsync();                       

        return await Db.Repayments.AsNoTracking()
                       .Where(r => r.ApplicationId == applicationId)
                       .OrderBy(r => r.InstallmentNo)
                       .ToListAsync();
    }

    public Task<Repayment?> GetByIdAsync(int repaymentId) =>
        Db.Repayments.Include(r => r.Application)
          .FirstOrDefaultAsync(r => r.RepaymentId == repaymentId);

    public Task<Repayment?> GetFirstUnpaidAsync(int applicationId) =>
        Db.Repayments.AsNoTracking()
          .Where(r => r.ApplicationId == applicationId && r.Status != "Paid")
          .OrderBy(r => r.InstallmentNo)
          .FirstOrDefaultAsync();


    public async Task<Repayment> UpdateStatusAsync(Repayment repayment, string status, string? paidVia = null)
    {
        LoanApplication application = repayment.Application;

        if (status == "Paid")
        {
            var payment = new Payment
            {
                TransactionRef = LoanCalculator.NewTransactionRef(),
                ApplicationId = application.ApplicationId,
                UserId = application.UserId,
                Amount = repayment.AmountDue,
                Method = paidVia ?? "Branch",
                Type = "Installment",
                Status = "Success",
                Description = $"Installment {repayment.InstallmentNo} of application #{application.ApplicationId}"
            };
            Db.Payments.Add(payment);
            repayment.Payment = payment;                   

            Db.Transactions.Add(new Transaction
            {
                UserId = application.UserId,
                ApplicationId = application.ApplicationId,
                Type = "Repayment",
                Amount = repayment.AmountDue,
                Description = paidVia is null
                    ? $"Installment {repayment.InstallmentNo} paid for application #{application.ApplicationId} (receipt {payment.TransactionRef})"
                    : $"Installment {repayment.InstallmentNo} paid online ({paidVia}) for application #{application.ApplicationId} (receipt {payment.TransactionRef})"
            });
        }
        else if (repayment.PaymentId is int oldPaymentId)
        {
            Payment? old = await Db.Payments.FindAsync(oldPaymentId);
            if (old is not null && old.Type == "Installment") old.Status = "Reversed";
            repayment.PaymentId = null;
        }

        repayment.Status = status;
        repayment.AmountPaid = status == "Paid" ? repayment.AmountDue : 0;
        repayment.PaidAt = status == "Paid" ? DateTime.UtcNow : null;

        bool othersUnpaid = await Db.Repayments.AnyAsync(r =>
            r.ApplicationId == application.ApplicationId &&
            r.RepaymentId != repayment.RepaymentId &&
            r.Status != "Paid");

        if (status == "Paid" && !othersUnpaid)
            application.Status = "Closed";
        else if (application.Status == "Closed")
            application.Status = "Disbursed";

        await Db.SaveChangesAsync();                    
        return repayment;
    }

                                                 
    public async Task<PaymentReceiptDto> PayOffAsync(int applicationId, decimal payoffAmount, string method)
    {
        await using var dbTransaction = await Db.Database.BeginTransactionAsync();

        LoanApplication application = await Db.LoanApplications.FindAsync(applicationId)
            ?? throw new InvalidOperationException("Application was not found.");

        List<Repayment> unpaid = await Db.Repayments
            .Where(r => r.ApplicationId == applicationId && r.Status != "Paid")
            .OrderBy(r => r.InstallmentNo)
            .ToListAsync();

        var payment = new Payment
        {
            TransactionRef = LoanCalculator.NewTransactionRef(),
            ApplicationId = applicationId,
            UserId = application.UserId,
            Amount = payoffAmount,
            Method = method,
            Type = "EarlyPayoff",
            Status = "Success",
            Description = $"Early repayment of application #{applicationId} ({unpaid.Count} installments settled)"
        };
        Db.Payments.Add(payment);

        decimal share = Math.Round(payoffAmount / unpaid.Count, 2, MidpointRounding.AwayFromZero);
        decimal allocated = 0;
        DateTime now = DateTime.UtcNow;
        for (int i = 0; i < unpaid.Count; i++)
        {
            decimal amount = i == unpaid.Count - 1 ? payoffAmount - allocated : share;
            allocated += amount;

            unpaid[i].AmountDue = amount;                  
            unpaid[i].AmountPaid = amount;
            unpaid[i].Status = "Paid";
            unpaid[i].PaidAt = now;
            unpaid[i].Payment = payment;
        }

        application.Status = "Closed";

        Db.Transactions.Add(new Transaction
        {
            UserId = application.UserId,
            ApplicationId = applicationId,
            Type = "Repayment",
            Amount = payoffAmount,
            Description = $"Loan #{applicationId} repaid early online ({method}), receipt {payment.TransactionRef}"
        });

        await Db.SaveChangesAsync();
        await dbTransaction.CommitAsync();                 

        return (await GetReceiptAsync(payment.PaymentId))!;
    }

    public Task<PaymentReceiptDto?> GetReceiptAsync(int paymentId) =>
        ToReceipt(Db.Payments.Where(p => p.PaymentId == paymentId)).FirstOrDefaultAsync();

    public Task<List<PaymentReceiptDto>> GetReceiptsByApplicationAsync(int applicationId) =>
        ToReceipt(Db.Payments.Where(p => p.ApplicationId == applicationId))
          .OrderByDescending(p => p.PaidAt)
          .ToListAsync();

    private static IQueryable<PaymentReceiptDto> ToReceipt(IQueryable<Payment> query) =>
        query.AsNoTracking().Select(p => new PaymentReceiptDto
        {
            PaymentId = p.PaymentId,
            TransactionId = p.TransactionRef,
            ApplicationId = p.ApplicationId,
            ProductName = p.Application.Product.Name,
            CustomerName = p.User.FullName,
            Amount = p.Amount,
            Method = p.Method,
            Type = p.Type,
            Status = p.Status,
            Description = p.Description,
            PaidAt = p.PaidAt,
            InstallmentNos = p.Repayments.OrderBy(r => r.InstallmentNo).Select(r => r.InstallmentNo).ToList()
        });
}