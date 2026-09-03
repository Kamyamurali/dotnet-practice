using Banking;

List<Accounts> accounts = new List<Accounts>();

bool running = true;
while (running)
{
    Console.WriteLine("\n====== MAIN MENU ======");
    Console.WriteLine("1. Create new account");
    Console.WriteLine("2. Check balance");
    Console.WriteLine("3. Withdraw");
    Console.WriteLine("4. Deposit");
    Console.WriteLine("0. Exit");
    Console.Write("Choose: ");
    int choice = Convert.ToInt32(Console.ReadLine());

    if (choice == 1)
    {
        Console.WriteLine("\n--- Account type ---");
        Console.WriteLine("1. Savings");
        Console.WriteLine("2. Checking");
        Console.WriteLine("3. Loans");
        Console.WriteLine("4. Back (to main menu)");
        Console.Write("Choose: ");
        int type = Convert.ToInt32(Console.ReadLine());

        if (type == 4) { continue; }         
        if (type < 1 || type > 3)
        {
            Console.WriteLine("Unknown type.");
            continue;
        }

        Console.Write("Account number: ");
        int no = Convert.ToInt32(Console.ReadLine());
        Console.Write("Account holder name: ");
        string name = Console.ReadLine();
        Console.Write("Starting balance: ");
        int bal = Convert.ToInt32(Console.ReadLine());

        Accounts newAcc;
        if (type == 1)
        {
            newAcc = new Savings()
            {
                AccountNumber = no, AccountHolderName = name,
                AccountType = TypeOfAccount.Savings,
                AccountBalance = bal, IsAccActive = true
            };
        }
        else if (type == 2)
        {
            Console.Write("Enable overdraft? (y/n): ");
            string od = Console.ReadLine();
            newAcc = new Checking()
            {
                AccountNumber = no, AccountHolderName = name,
                AccountType = TypeOfAccount.Checking,
                AccountBalance = bal, IsAccActive = true,
                isODEnabled = (od == "y" || od == "Y")
            };
        }
        else
        {
            newAcc = new Loans()
            {
                AccountNumber = no, AccountHolderName = name,
                AccountType = TypeOfAccount.Loans,
                AccountBalance = bal, IsAccActive = true
            };
        }

        accounts.Add(newAcc);
        SaveAccount(newAcc);   
        Console.WriteLine($"{newAcc.AccountType} account created and saved to {newAcc.AccountNumber}.txt!");
    }
    else if (choice == 0)
    {
        running = false;
    }
    else if (choice == 2 || choice == 3 || choice == 4)
    {
        if (accounts.Count == 0)
        {
            Console.WriteLine("No accounts yet - create one first (option 1).");
            continue;
        }

        // show accounts, then pick one
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

        Accounts acc = accounts[pick - 1];   

        try
        {
            if (choice == 2)
            {
                Console.WriteLine("Available balance: " + acc.CheckBalance());
            }
            else if (choice == 3)
            {
                Console.Write("Amount to withdraw: ");
                int amt = Convert.ToInt32(Console.ReadLine());
                acc.Withdraw(amt);       // savings / checking / loan react differently
                SaveAccount(acc);        // keep the file in sync
                Console.WriteLine("New balance: " + acc.CheckBalance());
            }
            else
            {
                Console.Write("Amount to deposit: ");
                int amt = Convert.ToInt32(Console.ReadLine());
                acc.Deposit(amt);
                SaveAccount(acc);
                Console.WriteLine("New balance: " + acc.CheckBalance());
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

void SaveAccount(Accounts a)
{
    List<string> lines = new List<string>();
    lines.Add("Account Number: " + a.AccountNumber);
    lines.Add("Account Holder: " + a.AccountHolderName);
    lines.Add("Account Type: " + a.AccountType);
    lines.Add("Balance: " + a.AccountBalance);
    lines.Add("Active: " + a.IsAccActive);

    if (a is Checking c)
    {
        lines.Add("OD Enabled: " + c.isODEnabled);
    }

    File.WriteAllLines(a.AccountNumber + ".txt", lines);
}