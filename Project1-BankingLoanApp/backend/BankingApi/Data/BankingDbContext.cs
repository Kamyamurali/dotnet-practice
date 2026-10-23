using BankingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Data;


public class BankingDbContext : DbContext
{
    public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<LoanProduct> LoanProducts => Set<LoanProduct>();
    public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();
    public DbSet<Disbursement> Disbursements => Set<Disbursement>();
    public DbSet<Repayment> Repayments => Set<Repayment>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Payment> Payments => Set<Payment>();

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.UserId);
            e.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            e.Property(u => u.Email).HasMaxLength(150).IsRequired();
            e.Property(u => u.Phone).HasMaxLength(20);
            e.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();
            e.Property(u => u.Role).HasMaxLength(20).IsRequired().HasDefaultValue("Customer");
            e.Property(u => u.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(u => u.Email).IsUnique();                      
            e.ToTable(t => t.HasCheckConstraint("CK_Users_Role", "[Role] IN ('Customer', 'Admin')"));
        });

        modelBuilder.Entity<LoanProduct>(e =>
        {
            e.HasKey(p => p.ProductId);
            e.Property(p => p.Name).HasMaxLength(100).IsRequired();
            e.Property(p => p.Category).HasMaxLength(50).IsRequired();
            e.Property(p => p.Description).HasMaxLength(500);
            e.Property(p => p.InterestRate).HasPrecision(5, 2);
            e.Property(p => p.MinAmount).HasPrecision(18, 2);         
            e.Property(p => p.MaxAmount).HasPrecision(18, 2);
            e.Property(p => p.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Products_Rate", "[InterestRate] >= 0 AND [InterestRate] <= 50");
                t.HasCheckConstraint("CK_Products_Amount", "[MinAmount] > 0 AND [MaxAmount] >= [MinAmount]");
                t.HasCheckConstraint("CK_Products_Tenure", "[MaxTenureMonths] BETWEEN 1 AND 480");
            });
        });

        modelBuilder.Entity<LoanApplication>(e =>
        {
            e.HasKey(a => a.ApplicationId);
            e.Property(a => a.AmountRequested).HasPrecision(18, 2);
            e.Property(a => a.InterestRate).HasPrecision(5, 2);
            e.Property(a => a.Purpose).HasMaxLength(300).IsRequired();
            e.Property(a => a.Status).HasMaxLength(20).IsRequired().HasDefaultValue("Pending");
            e.Property(a => a.AdminRemarks).HasMaxLength(300);
            e.Property(a => a.ApplicantDateOfBirth).HasColumnType("date");
            e.Property(a => a.MonthlyIncome).HasPrecision(18, 2);
            e.Property(a => a.AppliedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasOne(a => a.User).WithMany(u => u.Applications)
             .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Restrict);

            e.HasOne(a => a.Product).WithMany(p => p.Applications)
             .HasForeignKey(a => a.ProductId).OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(a => a.UserId);
            e.HasIndex(a => a.Status);

            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Applications_Amount", "[AmountRequested] > 0");
                t.HasCheckConstraint("CK_Applications_Tenure", "[TenureMonths] > 0");
                t.HasCheckConstraint("CK_Applications_Income", "[MonthlyIncome] IS NULL OR [MonthlyIncome] > 0");
                t.HasCheckConstraint("CK_Applications_Status",
                    "[Status] IN ('Pending', 'Approved', 'Rejected', 'Disbursed', 'Closed')");
            });
        });

        // ---------- Disbursements ----------
        modelBuilder.Entity<Disbursement>(e =>
        {
            e.HasKey(d => d.DisbursementId);
            e.Property(d => d.Amount).HasPrecision(18, 2);
            e.Property(d => d.Reference).HasMaxLength(50).IsRequired();
            e.Property(d => d.DisbursedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasOne(d => d.Application).WithOne(a => a.Disbursement)
             .HasForeignKey<Disbursement>(d => d.ApplicationId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(d => d.ApplicationId).IsUnique();              

            e.ToTable(t => t.HasCheckConstraint("CK_Disbursements_Amount", "[Amount] > 0"));
        });

        modelBuilder.Entity<Repayment>(e =>
        {
            e.HasKey(r => r.RepaymentId);
            e.Property(r => r.DueDate).HasColumnType("date");
            e.Property(r => r.AmountDue).HasPrecision(18, 2);
            e.Property(r => r.AmountPaid).HasPrecision(18, 2);
            e.Property(r => r.Status).HasMaxLength(20).IsRequired().HasDefaultValue("Due");

            e.HasOne(r => r.Application).WithMany(a => a.Repayments)
             .HasForeignKey(r => r.ApplicationId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(r => new { r.ApplicationId, r.InstallmentNo }).IsUnique();

            e.HasOne(r => r.Payment).WithMany(p => p.Repayments)
             .HasForeignKey(r => r.PaymentId).OnDelete(DeleteBehavior.Restrict);

            e.ToTable(t => t.HasCheckConstraint("CK_Repayments_Status", "[Status] IN ('Due', 'Paid', 'Overdue')"));
        });

        modelBuilder.Entity<Payment>(e =>
        {
            e.HasKey(p => p.PaymentId);
            e.Property(p => p.TransactionRef).HasMaxLength(30).IsRequired();
            e.Property(p => p.Amount).HasPrecision(18, 2);
            e.Property(p => p.Method).HasMaxLength(20).IsRequired();
            e.Property(p => p.Type).HasMaxLength(20).IsRequired();
            e.Property(p => p.Status).HasMaxLength(20).IsRequired().HasDefaultValue("Success");
            e.Property(p => p.Description).HasMaxLength(300).IsRequired();
            e.Property(p => p.PaidAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasOne(p => p.Application).WithMany()
             .HasForeignKey(p => p.ApplicationId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.User).WithMany()
             .HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(p => p.TransactionRef).IsUnique();             
            e.HasIndex(p => p.ApplicationId);

            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Payments_Amount", "[Amount] > 0");
                t.HasCheckConstraint("CK_Payments_Method", "[Method] IN ('Card', 'UPI', 'NetBanking', 'Branch')");
                t.HasCheckConstraint("CK_Payments_Type", "[Type] IN ('Installment', 'EarlyPayoff')");
                t.HasCheckConstraint("CK_Payments_Status", "[Status] IN ('Success', 'Reversed')");
            });
        });

        modelBuilder.Entity<Transaction>(e =>
        {
            e.HasKey(t => t.TransactionId);
            e.Property(t => t.Type).HasMaxLength(30).IsRequired();
            e.Property(t => t.Amount).HasPrecision(18, 2);
            e.Property(t => t.Description).HasMaxLength(300).IsRequired();
            e.Property(t => t.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasOne(t => t.User).WithMany(u => u.Transactions)
             .HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.Application).WithMany()
             .HasForeignKey(t => t.ApplicationId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(t => t.UserId);

            e.ToTable(t => t.HasCheckConstraint("CK_Transactions_Type",
                "[Type] IN ('Registration', 'Application', 'Approval', 'Rejection', 'Disbursement', 'Repayment')"));
        });

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                if (property.Name is nameof(Repayment.DueDate) or nameof(LoanApplication.ApplicantDateOfBirth)) continue;

                if (property.ClrType == typeof(DateTime))
                    property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                        v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                else if (property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>(
                        v => v, v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v));
            }
        }
    }
}