namespace collection_good_task
{
    class Program
    {
        static List<SavingsAccount> accounts = new List<SavingsAccount>();

        static void Main()
        {
            SeedAccounts();   

            while (true)
            {
                Console.WriteLine("\n===== BANK MENU =====");
                Console.WriteLine("1. Add New Account");
                Console.WriteLine("2. View Account Details");
                Console.WriteLine("3. Withdraw");
                Console.WriteLine("4. Deposit");
                Console.WriteLine("5. Transfer");
                Console.WriteLine("6. Summary");
                Console.WriteLine("7. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();

                if (choice == "1") AddNewAccount();
                else if (choice == "2") ViewAccountDetails();
                else if (choice == "3") Withdraw();
                else if (choice == "4") Deposit();
                else if (choice == "5") Transfer();
                else if (choice == "6") Summary();
                else if (choice == "7") { Console.WriteLine("Goodbye!"); break; }
                else Console.WriteLine("Invalid choice, try again.");
            }
        }

        static SavingsAccount FindAccount(int accNo)
        {
            foreach (SavingsAccount acc in accounts)
            {
                if (acc.accNo == accNo)
                    return acc;      
            }
            return null;             
        }

        static void SeedAccounts()
        {
            accounts.Add(new SavingsAccount { accNo = 1001, accName = "Arjun Menon",     accBalance = 15000, accIsActive = true,  accBranch = "Chennai"   });
            accounts.Add(new SavingsAccount { accNo = 1002, accName = "Priya Sharma",    accBalance = 32000, accIsActive = true,  accBranch = "Bangalore" });
            accounts.Add(new SavingsAccount { accNo = 1003, accName = "Rahul Nair",      accBalance = 8500,  accIsActive = true,  accBranch = "Chennai"   });
            accounts.Add(new SavingsAccount { accNo = 1004, accName = "Sneha Reddy",     accBalance = 47000, accIsActive = false, accBranch = "Hyderabad" });
            accounts.Add(new SavingsAccount { accNo = 1005, accName = "Vikram Singh",    accBalance = 12000, accIsActive = true,  accBranch = "Mumbai"    });
            accounts.Add(new SavingsAccount { accNo = 1006, accName = "Ananya Iyer",     accBalance = 65000, accIsActive = true,  accBranch = "Chennai"   });
            accounts.Add(new SavingsAccount { accNo = 1007, accName = "Karthik Rao",     accBalance = 5000,  accIsActive = true,  accBranch = "Bangalore" });
            accounts.Add(new SavingsAccount { accNo = 1008, accName = "Divya Pillai",    accBalance = 28000, accIsActive = false, accBranch = "Kochi"     });
            accounts.Add(new SavingsAccount { accNo = 1009, accName = "Rohan Gupta",     accBalance = 90000, accIsActive = true,  accBranch = "Delhi"     });
            accounts.Add(new SavingsAccount { accNo = 1010, accName = "Meera Krishnan",  accBalance = 3500,  accIsActive = true,  accBranch = "Chennai"   });
            accounts.Add(new SavingsAccount { accNo = 1011, accName = "Aditya Verma",    accBalance = 21000, accIsActive = true,  accBranch = "Pune"      });
            accounts.Add(new SavingsAccount { accNo = 1012, accName = "Nisha Joshi",     accBalance = 17500, accIsActive = false, accBranch = "Mumbai"    });
            accounts.Add(new SavingsAccount { accNo = 1013, accName = "Sanjay Kumar",    accBalance = 42000, accIsActive = true,  accBranch = "Hyderabad" });
            accounts.Add(new SavingsAccount { accNo = 1014, accName = "Pooja Desai",     accBalance = 9800,  accIsActive = true,  accBranch = "Bangalore" });
            accounts.Add(new SavingsAccount { accNo = 1015, accName = "Aravind Balaji",  accBalance = 55000, accIsActive = true,  accBranch = "Chennai"   });
        }

        static void AddNewAccount()
        {
            Console.Write("Enter account number: ");
            int no = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter account holder name: ");
            string name = Console.ReadLine();

            Console.Write("Enter opening balance: ");
            double bal = Convert.ToDouble(Console.ReadLine());

            Console.Write("Is the account active? (yes/no): ");
            bool active = (Console.ReadLine() == "yes");

            Console.Write("Enter branch: ");
            string branch = Console.ReadLine();

            SavingsAccount newAcc = new SavingsAccount
            {
                accNo = no,
                accName = name,
                accBalance = bal,
                accIsActive = active,
                accBranch = branch
            };

            accounts.Add(newAcc);
            Console.WriteLine("Account added successfully!");
        }

        static void ViewAccountDetails()
        {
            Console.WriteLine();
            Console.WriteLine("AccNo".PadRight(8) + "Name".PadRight(18) +
                              "Balance".PadRight(12) + "Active".PadRight(8) + "Branch");
            Console.WriteLine(new string('-', 55));

            foreach (SavingsAccount acc in accounts)
            {
                Console.WriteLine(
                    acc.accNo.ToString().PadRight(8) +
                    acc.accName.PadRight(18) +
                    acc.accBalance.ToString().PadRight(12) +
                    (acc.accIsActive ? "Yes" : "No").PadRight(8) +
                    acc.accBranch);
            }
        }

        static void Withdraw()
        {
            Console.Write("Enter account number: ");
            int no = Convert.ToInt32(Console.ReadLine());

            SavingsAccount acc = FindAccount(no);
            if (acc == null) { Console.WriteLine("Account not found."); return; }

            Console.Write("Enter amount to withdraw: ");
            double amount = Convert.ToDouble(Console.ReadLine());

            if (amount > acc.accBalance)
            {
                Console.WriteLine("Not enough balance.");
                return;
            }

            acc.accBalance = acc.accBalance - amount;
            Console.WriteLine("Withdrawn. New balance: " + acc.accBalance);
        }

        static void Deposit()
        {
            Console.Write("Enter account number: ");
            int no = Convert.ToInt32(Console.ReadLine());

            SavingsAccount acc = FindAccount(no);
            if (acc == null) { Console.WriteLine("Account not found."); return; }

            Console.Write("Enter amount to deposit: ");
            double amount = Convert.ToDouble(Console.ReadLine());

            acc.accBalance = acc.accBalance + amount;
            Console.WriteLine("Deposited. New balance: " + acc.accBalance);
        }

        static void Transfer()
        {
            Console.Write("Transfer FROM account number: ");
            int fromNo = Convert.ToInt32(Console.ReadLine());
            SavingsAccount fromAcc = FindAccount(fromNo);
            if (fromAcc == null) { Console.WriteLine("From-account not found."); return; }

            Console.Write("Transfer TO account number: ");
            int toNo = Convert.ToInt32(Console.ReadLine());
            SavingsAccount toAcc = FindAccount(toNo);
            if (toAcc == null) { Console.WriteLine("To-account not found."); return; }

            Console.Write("Enter amount to transfer: ");
            double amount = Convert.ToDouble(Console.ReadLine());

            if (amount > fromAcc.accBalance)
            {
                Console.WriteLine("Not enough balance to transfer.");
                return;
            }

            fromAcc.accBalance = fromAcc.accBalance - amount;
            toAcc.accBalance = toAcc.accBalance + amount;

            Console.WriteLine("Transfer done.");
            Console.WriteLine(fromAcc.accName + " new balance: " + fromAcc.accBalance);
            Console.WriteLine(toAcc.accName + " new balance: " + toAcc.accBalance);
        }

        static void Summary()
        {
            while (true)
            {
                Console.WriteLine("\n--- Summary ---");
                Console.WriteLine("a. Total Accounts");
                Console.WriteLine("b. Total Balance with Bank");
                Console.WriteLine("c. Total Active Accounts");
                Console.WriteLine("d. Total Inactive Accounts");
                Console.WriteLine("e. Back");
                Console.Write("Choose: ");

                string choice = Console.ReadLine();

                if (choice == "a")
                {
                    Console.WriteLine("Total accounts: " + accounts.Count);
                }
                else if (choice == "b")
                {
                    double total = 0;
                    foreach (SavingsAccount acc in accounts)
                        total = total + acc.accBalance;
                    Console.WriteLine("Total balance with bank: " + total);
                }
                else if (choice == "c")
                {
                    int activeCount = 0;
                    foreach (SavingsAccount acc in accounts)
                        if (acc.accIsActive)
                            activeCount++;
                    Console.WriteLine("Total active accounts: " + activeCount);
                }
                else if (choice == "d")
                {
                    int inactiveCount = 0;
                    foreach (SavingsAccount acc in accounts)
                        if (!acc.accIsActive)
                            inactiveCount++;
                    Console.WriteLine("Total inactive accounts: " + inactiveCount);
                }
                else if (choice == "e")
                {
                    break;   
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                }
            }
        }
    }
}

