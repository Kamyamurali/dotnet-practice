namespace OOPS_bankingAPP
{
    public class Accounts
    {        


        #region Properties
        public int AccNo { get; set; } //auto implemented property
        public string AccName { get; set; } //auto implemented property
        public double AccBalance { get; set; } //auto implemented property
        public bool IsActive { get; set; } //auto implemented property
        public string email { get; set; } //auto implemented property
#endregion

        #region  Methods
    public double Withdraw(int amount)
    {
        AccBalance = AccBalance - amount;
        return AccBalance;
    }

    public double Deposit(int amount)
    {
        AccBalance = AccBalance + amount;
        return AccBalance;
    }
#endregion
   
    
   
    }
}