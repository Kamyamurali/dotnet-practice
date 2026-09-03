namespace Banking
{
    public class Loans : Accounts
    {
        public override double Withdraw(int amount)
        {
            throw new Exception("Sorry withdrawal not allow, please contact bank");
        }
    }
}