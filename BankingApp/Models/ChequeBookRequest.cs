namespace BankingApp.Models;

public class ChequeBookRequest
{
    public int ChequeBookRequestId { get; set; }
    public DateTime RequestDate { get; set; } = DateTime.Now;
    public bool IsApproved { get; set; }

    public int AccountId { get; set; }
    public Account? Account { get; set; }
}