using BankingApi.DTOs;
using BankingApi.Repositories;
using BankingApi.Security;
using BankingApi.Services;

namespace BankingApi.Data;


// Test logins:
//   Admin     admin@bank.com      Admin@123
//   Customer  priya@example.com   Customer@123
//   Customer  rahul@example.com   Customer@123
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var users = services.GetRequiredService<IUserRepository>();
        if (await users.AnyUsersAsync()) return;               

        var hasher = services.GetRequiredService<IPasswordHasher>();
        var auth = services.GetRequiredService<IAuthService>();
        var products = services.GetRequiredService<IProductService>();
        var applications = services.GetRequiredService<IApplicationService>();
        var disbursements = services.GetRequiredService<IDisbursementService>();
        var repayments = services.GetRequiredService<IRepaymentService>();

        await users.RegisterAsync("Bank Administrator", "admin@bank.com", "2025550100",
                                  hasher.Hash("Admin@123"), role: "Admin");
        var priya = await auth.RegisterAsync(new RegisterRequest
            { FullName = "Priya Sharma", Email = "priya@example.com", Phone = "2025550101", Password = "Customer@123" });
        var rahul = await auth.RegisterAsync(new RegisterRequest
            { FullName = "Rahul Verma", Email = "rahul@example.com", Phone = "2025550102", Password = "Customer@123" });

        var home = await products.AddAsync(Product("Home Loan", "Home",
            "Buy or build a home with long repayment terms.", 8.50m, 50000, 2000000, 240));
        var education = await products.AddAsync(Product("Education Loan", "Education",
            "Tuition, books and living costs for higher studies.", 6.75m, 5000, 150000, 120));
        var personal = await products.AddAsync(Product("Personal Loan", "Personal",
            "Quick funds for personal needs, no collateral.", 11.25m, 1000, 50000, 60));
        var vehicle = await products.AddAsync(Product("Vehicle Loan", "Vehicle",
            "Finance a new or used car or two-wheeler.", 9.10m, 5000, 100000, 84));
        await products.AddAsync(Product("Small Business Loan", "Business",
            "Working capital and equipment for small businesses.", 10.00m, 10000, 500000, 120));

        int priyaId = priya.User.UserId, rahulId = rahul.User.UserId;
        var priyaDob = new DateTime(1998, 4, 12); const decimal priyaIncome = 6000;
        var rahulDob = new DateTime(1995, 9, 3);  const decimal rahulIncome = 18000;

        var app1 = await applications.ApplyAsync(priyaId, Apply(personal.ProductId, 12000, 12, "Home renovation", priyaDob, priyaIncome));
        await applications.ReviewAsync(app1.ApplicationId, Review("Approved", "Good repayment capacity"));
        await disbursements.DisburseAsync(app1.ApplicationId);
        var schedule = await repayments.GetByApplicationAsync(app1.ApplicationId, priyaId, isAdmin: false);
        await repayments.UpdateStatusAsync(schedule[0].RepaymentId, "Paid");
        await repayments.UpdateStatusAsync(schedule[1].RepaymentId, "Paid");

        await applications.ApplyAsync(priyaId, Apply(education.ProductId, 40000, 60, "Master's degree tuition", priyaDob, priyaIncome));

        var app3 = await applications.ApplyAsync(rahulId, Apply(vehicle.ProductId, 25000, 36, "Buying a used car", rahulDob, rahulIncome));
        await applications.ReviewAsync(app3.ApplicationId, Review("Approved", "Documents verified"));

        var app4 = await applications.ApplyAsync(rahulId, Apply(home.ProductId, 900000, 240, "Buying an apartment", rahulDob, rahulIncome));
        await applications.ReviewAsync(app4.ApplicationId, Review("Rejected", "Income too low for this amount"));

        await applications.ApplyAsync(rahulId, Apply(personal.ProductId, 5000, 24, "Medical expenses", rahulDob, rahulIncome));
    }

    private static ProductRequest Product(string name, string category, string description,
        decimal rate, decimal min, decimal max, int months) => new()
    {
        Name = name, Category = category, Description = description,
        InterestRate = rate, MinAmount = min, MaxAmount = max, MaxTenureMonths = months, IsActive = true
    };

    private static ApplyLoanRequest Apply(int productId, decimal amount, int months, string purpose,
        DateTime dateOfBirth, decimal monthlyIncome) => new()
        { ProductId = productId, AmountRequested = amount, TenureMonths = months, Purpose = purpose,
          DateOfBirth = dateOfBirth, MonthlyIncome = monthlyIncome };

    private static ReviewApplicationRequest Review(string status, string remarks) => new()
        { Status = status, AdminRemarks = remarks };
}