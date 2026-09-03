using Banking;

// Starts empty. Every account YOU open gets stored here.
List<Accounts> accounts = new List<Accounts>();

bool running = true;
while (running)
{
    Console.WriteLine("\n====== BANK MENU ======");
    Console.WriteLine("1. Open a new account");
    Console.WriteLine("2. Withdraw");
    Console.WriteLine("3. Deposit");
    Console.WriteLine("4. Check balance");
    Console.WriteLine("0. Exit");
    Console.Write("Choose: ");
    int choice = Convert.ToInt32(Console.ReadLine());

    // ----- OPEN A NEW ACCOUNT -----
    if (choice == 1)
    {
        Console.Write("Type  1. Savings  2. Checking  3. Loan : ");
        int type = Convert.ToInt32(Console.ReadLine());
        Console.Write("Account number: ");
        int no = Convert.ToInt32(Console.ReadLine());
        Console.Write("Account holder name: ");
        string name = Console.ReadLine();
        Console.Write("Starting balance: ");
        int bal = Convert.ToInt32(Console.ReadLine());

        if (type == 1)
        {
            accounts.Add(new Savings()
            {
                AccountNumber = no, AccountHolderName = name,
                AccountType = TypeOfAccount.Savings,
                AccountBalance = bal, IsAccActive = true
            });
            Console.WriteLine("Savings account created!");
        }
        else if (type == 2)
        {
            Console.Write("Enable overdraft? (y/n): ");
            string od = Console.ReadLine();
            accounts.Add(new Checking()
            {
                AccountNumber = no, AccountHolderName = name,
                AccountType = TypeOfAccount.Checking,
                AccountBalance = bal, IsAccActive = true,
                isODEnabled = (od == "y" || od == "Y")
            });
            Console.WriteLine("Checking account created!");
        }
        else if (type == 3)
        {
            accounts.Add(new Loans()
            {
                AccountNumber = no, AccountHolderName = name,
                AccountType = TypeOfAccount.Loans,
                AccountBalance = bal, IsAccActive = true
            });
            Console.WriteLine("Loan account created!");
        }
        else
        {
            Console.WriteLine("Unknown type.");
        }
    }
    // ----- EXIT -----
    else if (choice == 0)
    {
        running = false;
    }
    // ----- WITHDRAW / DEPOSIT / CHECK BALANCE -----
    else if (choice == 2 || choice == 3 || choice == 4)
    {
        if (accounts.Count == 0)
        {
            Console.WriteLine("No accounts yet - open one first (option 1).");
            continue;
        }

        // Show what you've opened, then pick one.
        Console.WriteLine("Your accounts:");
        for (int i = 0; i < accounts.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {accounts[i].AccountHolderName} ({accounts[i].AccountType}) - balance {accounts[i].AccountBalance}");
        }
        Console.Write("Pick account: ");
        int pick = Convert.ToInt32(Console.ReadLine());
        if (pick < 1 || pick > accounts.Count)
        {
            Console.WriteLine("Invalid pick.");
            continue;
        }

        Accounts acc = accounts[pick - 1];   // base-type reference

        // Each action is wrapped in try/catch because the class methods
        // throw an Exception when a rule is broken.
        try
        {
            if (choice == 2)
            {
                Console.Write("Amount to withdraw: ");
                int amt = Convert.ToInt32(Console.ReadLine());
                acc.Withdraw(amt);   // savings / checking / loan each react differently
                Console.WriteLine("New balance: " + acc.CheckBalance());
            }
            else if (choice == 3)
            {
                Console.Write("Amount to deposit: ");
                int amt = Convert.ToInt32(Console.ReadLine());
                acc.Deposit(amt);
                Console.WriteLine("New balance: " + acc.CheckBalance());
            }
            else
            {
                Console.WriteLine("Available balance: " + acc.CheckBalance());
            }
        }
        catch (Exception es)
        {
            Console.WriteLine(es.Message);
        }
    }
    else
    {
        Console.WriteLine("Invalid choice.");
    }
}

Console.WriteLine("Goodbye!");