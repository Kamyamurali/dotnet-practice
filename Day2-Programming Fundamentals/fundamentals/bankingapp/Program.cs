double balance = 5000.00;

bool continueTransaction = true;

while (continueTransaction)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("!~~~~~~~~~~~~~~~~~~~~ Bank of America 2.0 ~~~~~~~~~~~~~~~~~~~~!");
    Console.ResetColor();
    Console.WriteLine("Current Balance: $" + balance);
    Console.WriteLine();
    Console.WriteLine("1. Create Account");
    Console.WriteLine("2. Check Balance");
    Console.WriteLine("3. Withdraw Funds");
    Console.WriteLine("4. Deposit Funds");
    Console.WriteLine("5. Transfer Funds");
    Console.WriteLine("6. View Transaction History");
    Console.WriteLine("7. Change ATM PIN");
    Console.WriteLine("8. Request Loan");
    Console.WriteLine("9. Exit");
    Console.WriteLine();

    Console.Write("Enter your choice: ");
    int userChoice = Convert.ToInt32(Console.ReadLine());

    switch (userChoice)
    {
        case 1:
            Console.WriteLine("Account created successfully!");
            break;

        case 2:
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Your current balance is: $" + balance);
            Console.ResetColor();
            break;

        case 3:
            Console.Write("Enter amount to withdraw: $");
            double withdrawAmount = Convert.ToDouble(Console.ReadLine());

            if (withdrawAmount > balance)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Insufficient funds.");
                Console.ResetColor();
            }
            else
            {
                balance = balance - withdrawAmount;
                Console.WriteLine("Withdrew $" + withdrawAmount + ". New balance: $" + balance);
            }
            break;

        case 4:
            Console.Write("Enter amount to deposit: $");
            double depositAmount = Convert.ToDouble(Console.ReadLine());
            balance = balance + depositAmount;
            Console.WriteLine("Deposited $" + depositAmount + ". New balance: $" + balance);
            break;

        case 5:
            Console.WriteLine("Transferring funds.");
            break;

        case 6:
            Console.WriteLine("Viewing transaction history.");
            break;

        case 7:
            Console.WriteLine("Changing ATM PIN.");
            break;

        case 8:
            Console.WriteLine("Requesting loan.");
            break;

        case 9:
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Thank you for using Bank of America. Goodbye!");
            Console.ResetColor();
            continueTransaction = false;
            break;

        default:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid choice. Please try again.");
            Console.ResetColor();
            break;
    }

    if (continueTransaction)
    {
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}