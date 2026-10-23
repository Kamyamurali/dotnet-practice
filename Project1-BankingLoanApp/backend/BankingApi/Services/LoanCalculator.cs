namespace BankingApi.Services;

public static class LoanCalculator
{
    // EMI = P * r * (1+r)^n / ((1+r)^n - 1), where r = yearly rate / 12 / 100
    public static decimal MonthlyInstallment(decimal principal, decimal yearlyRate, int months)
    {
        if (months <= 0) throw new ArgumentOutOfRangeException(nameof(months));
        if (yearlyRate == 0) return Math.Round(principal / months, 2, MidpointRounding.AwayFromZero);

        double r = (double)yearlyRate / 1200.0;
        double factor = Math.Pow(1 + r, months);
        double emi = (double)principal * r * factor / (factor - 1);
        return Math.Round((decimal)emi, 2, MidpointRounding.AwayFromZero);
    }

 
    public static decimal RemainingPrincipal(decimal principal, decimal yearlyRate, int months, int installmentsPaid)
    {
        if (installmentsPaid <= 0) return principal;
        if (installmentsPaid >= months) return 0;

        decimal emi = MonthlyInstallment(principal, yearlyRate, months);
        double balance;
        if (yearlyRate == 0)
        {
            balance = (double)principal - (double)emi * installmentsPaid;
        }
        else
        {
            double r = (double)yearlyRate / 1200.0;
            double growth = Math.Pow(1 + r, installmentsPaid);
            balance = (double)principal * growth - (double)emi * (growth - 1) / r;
        }
        return Math.Max(0, Math.Round((decimal)balance, 2, MidpointRounding.AwayFromZero));
    }

    public static string NewTransactionRef() =>
        "TXN-" + Guid.NewGuid().ToString("N")[..10].ToUpperInvariant();
}