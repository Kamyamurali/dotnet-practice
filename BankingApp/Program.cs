
using BankingApp.Data;
using BankingApp.Security;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

try
{
    using var db = new BankingDbContext();
    db.Database.Migrate();
    DbInitializer.Seed(db);
}
catch (Exception ex)
{
    Console.WriteLine("Could not connect to the database. Is Docker running?");
    Console.WriteLine("Details: " + ex.Message);
    return;   // stop the app cleanly instead of crashing
}

const string BankRoutingNumber = "021000021";

bool running = true;
while (running)
{
    Header("Welcome to the Bank");
    Console.WriteLine("1. Customer");
    Console.WriteLine("2. Admin");
    Console.WriteLine("3. Exit");

    string? choice = ReadLineOrEsc("Enter your choice: ");
    if (choice == null) { running = false; break; }   // Esc = exit the app

    switch (choice)
    {
        case "1":
            CustomerPortal();
            break;
        case "2":
            AdminLogin();
            break;
        case "3":
            Console.WriteLine("Thank you for banking with us.");
            running = false;
            break;
        default:
            Console.WriteLine("Invalid choice");
            Pause();
            break;
    }
}
void CustomerPortal()
{
    while (true)
    {
        Header("Customer");
        Console.WriteLine("1. Login (existing customer)");
        Console.WriteLine("2. Register (new customer)");
        Console.WriteLine("3. Back");

        string? choice = ReadLineOrEsc("Enter your choice: ");
        if (choice == null) return;   

        switch (choice)
        {
            case "1": CustomerLogin(); break;
            case "2": RegisterCustomer(); break;
            case "3": return;
            default: Console.WriteLine("Invalid choice"); Pause(); break;
        }
    }
}

void RegisterCustomer()
{
    using var db = new BankingDbContext();

    string? name = ReadRequired("Enter your full name: ");
    if (name == null) return;
    string? username = ReadRequired("Choose a username: ");
    if (username == null) return;
    if (db.Customers.Any(c => c.Username == username)) { Console.WriteLine("That username is already taken."); Pause(); return; }
    string? password = ReadNewPassword("Choose a password: ");
    if (password == null) return;
    string? email = ReadEmail("Enter your email: ");
    if (email == null) return;
    if (db.Customers.Any(c => c.Email == email)) { Console.WriteLine("A customer with this email already exists."); Pause(); return; }
    string? phone = ReadPhone("Enter your phone (10 digits): ");
    if (phone == null) return;
    if (db.Customers.Any(c => c.Phone == phone)) { Console.WriteLine("A customer with this phone number already exists."); Pause(); return; }

    var customer = new BankingApp.Models.Customer
    {
        FullName = name,
        Username = username,
        Password = PasswordHasher.Hash(password),
        Email = email,
        Phone = phone
    };
    db.Customers.Add(customer);
    db.SaveChanges();

    Console.WriteLine();
    Console.WriteLine("Registration successful! Now let's open your first account.");
    OpenAccount(db, customer);
    Pause();

    CustomerMenu(customer);   
}

void OpenAccount(BankingDbContext db, BankingApp.Models.Customer customer)
{
    string? type = ReadAccountType("Enter account type (Savings/Current): ");
    if (type == null) return;
    decimal? balanceInput = ReadAmount("Enter opening balance: ");
    if (balanceInput == null) return;
    decimal balance = balanceInput.Value;

    string accountNumber = GenerateAccountNumber(db);

    var account = new BankingApp.Models.Account
    {
        AccountNumber = accountNumber,
        AccountType = type,
        Balance = balance,
        CustomerId = customer.CustomerId
    };
    db.Accounts.Add(account);
    db.SaveChanges();

    Console.WriteLine();
    Console.WriteLine("Account opened successfully!");
    Console.WriteLine($"Account Number : {accountNumber}");
    Console.WriteLine($"Routing Number : {BankRoutingNumber}");
    Console.WriteLine($"Account Type   : {type}");
    Console.WriteLine($"Balance        : {balance:C}");
}

string GenerateAccountNumber(BankingDbContext db)
{
    string number;
    do
    {
        number = Random.Shared.NextInt64(1000000000L, 9999999999L).ToString();
    } while (db.Accounts.Any(a => a.AccountNumber == number));
    return number;
}

//  Customer Authentication 
void CustomerLogin()
{
    using var db = new BankingDbContext();
    int attempts = 0;          // wrong passwords so far
    const int MaxAttempts = 3;

    while (true)   
    {
        string? username = ReadLineOrEsc("Please enter username: ");
        if (username == null) return;   

        var customer = db.Customers.FirstOrDefault(c => c.Username == username);
        if (customer == null)
        {
            Console.WriteLine("No customer found with that username. Please try again (or press Esc to go back).");
            continue;   
        }

        while (true)
        {
            string? password = ReadLineOrEsc("Please enter Password: ", mask: true);
            if (password == null) break;  

            if (PasswordHasher.Verify(password, customer.Password))
            {
                // Old plain-text password? Upgrade it to a hash now that we know it's correct.
                if (!PasswordHasher.IsHashed(customer.Password))
                {
                    customer.Password = PasswordHasher.Hash(password);
                    db.SaveChanges();
                }
                CustomerMenu(customer);
                return; 
            }

            attempts++;
            if (attempts >= MaxAttempts)
            {
                Console.WriteLine("Too many failed attempts. Returning to the main menu.");
                Pause();
                return;
            }
            Console.WriteLine($"Incorrect password. {MaxAttempts - attempts} attempt(s) left (or press Esc to change username).");
        }
    }
}

//  Customer Menu 
void CustomerMenu(BankingApp.Models.Customer customer)
{
    bool loggedIn = true;
    while (loggedIn)
    {
        Header("Customer Menu");
        Console.WriteLine("1. Check Account Details");
        Console.WriteLine("2. Withdraw");
        Console.WriteLine("3. Deposit");
        Console.WriteLine("4. Transfer");
        Console.WriteLine("5. Last 5 Transactions");
        Console.WriteLine("6. Request Cheque Book");
        Console.WriteLine("7. Change Password");
        Console.WriteLine("8. Add Account");
        Console.WriteLine("9. Exit");

        string? choice = ReadLineOrEsc("Enter your choice: ");
        if (choice == null) { loggedIn = false; break; }   

        using var db = new BankingDbContext();

        switch (choice)
        {
            case "1": 
                {
                    var accounts = db.Accounts.Where(a => a.CustomerId == customer.CustomerId).ToList();
                    if (accounts.Count == 0)
                    {
                        Console.WriteLine("You have no accounts yet. Use 'Add Account' to open one.");
                        break;
                    }
                    foreach (var acc in accounts)
                    {
                        Console.WriteLine();
                        Console.WriteLine("=================================");
                        Console.WriteLine("        ACCOUNT DETAILS");
                        Console.WriteLine("=================================");
                        Console.WriteLine($"Customer ID     : {customer.CustomerId}");
                        Console.WriteLine($"Name            : {customer.FullName}");
                        Console.WriteLine($"Account Number  : {acc.AccountNumber}");
                        Console.WriteLine($"Routing Number  : {BankRoutingNumber}");
                        Console.WriteLine($"Account Type    : {acc.AccountType}");
                        Console.WriteLine($"Balance         : {acc.Balance:C}");
                        Console.WriteLine($"Email           : {customer.Email}");
                        Console.WriteLine($"Phone           : {customer.Phone}");
                    }
                }
                break;

            case "2": 
                {
                    string? accNo = ReadLineOrEsc("Enter account number: ");
                    if (accNo == null) break;
                    var account = db.Accounts.FirstOrDefault(a => a.AccountNumber == accNo && a.CustomerId == customer.CustomerId);
                    if (account == null) { Console.WriteLine("Account not found."); break; }
                    if (!account.IsActive) { Console.WriteLine("This account is inactive. Please contact the bank."); break; }

                    decimal? amountInput = ReadAmount("Enter amount to withdraw: ");
                    if (amountInput == null) break;
                    decimal amount = amountInput.Value;
                    if (amount > account.Balance) { Console.WriteLine("Insufficient balance."); break; }

                    account.Balance -= amount;
                    db.Transactions.Add(new BankingApp.Models.Transaction { AccountId = account.AccountId, Type = "Withdrawal", Amount = amount, BalanceAfter = account.Balance });
                    db.SaveChanges();
                    Console.WriteLine($"Withdrawal successful. New balance: {account.Balance:C}");
                }
                break;

            case "3": 
                {
                    string? accNo = ReadLineOrEsc("Enter account number: ");
                    if (accNo == null) break;
                    var account = db.Accounts.FirstOrDefault(a => a.AccountNumber == accNo && a.CustomerId == customer.CustomerId);
                    if (account == null) { Console.WriteLine("Account not found."); break; }
                    if (!account.IsActive) { Console.WriteLine("This account is inactive. Please contact the bank."); break; }

                    decimal? amountInput = ReadAmount("Enter amount to deposit: ");
                    if (amountInput == null) break;
                    decimal amount = amountInput.Value;

                    account.Balance += amount;
                    db.Transactions.Add(new BankingApp.Models.Transaction { AccountId = account.AccountId, Type = "Deposit", Amount = amount, BalanceAfter = account.Balance });
                    db.SaveChanges();
                    Console.WriteLine($"Deposit successful. New balance: {account.Balance:C}");
                }
                break;

            case "4": 
                {
                    string? fromNo = ReadLineOrEsc("Enter your account number: ");
                    if (fromNo == null) break;
                    var from = db.Accounts.FirstOrDefault(a => a.AccountNumber == fromNo && a.CustomerId == customer.CustomerId);
                    if (from == null) { Console.WriteLine("Account not found."); break; }
                    if (!from.IsActive) { Console.WriteLine("Your account is inactive. Please contact the bank."); break; }

                    string? toNo = ReadLineOrEsc("Enter destination account number: ");
                    if (toNo == null) break;
                    var to = db.Accounts.FirstOrDefault(a => a.AccountNumber == toNo);
                    if (to == null) { Console.WriteLine("Destination account not found."); break; }
                    if (to.AccountId == from.AccountId) { Console.WriteLine("Cannot transfer to the same account."); break; }
                    if (!to.IsActive) { Console.WriteLine("Destination account is inactive."); break; }

                    decimal? amountInput = ReadAmount("Enter amount to transfer: ");
                    if (amountInput == null) break;
                    decimal amount = amountInput.Value;
                    if (amount > from.Balance) { Console.WriteLine("Insufficient balance."); break; }

                    from.Balance -= amount;
                    to.Balance += amount;
                    db.Transactions.Add(new BankingApp.Models.Transaction { AccountId = from.AccountId, Type = "Transfer Out", Amount = amount, BalanceAfter = from.Balance });
                    db.Transactions.Add(new BankingApp.Models.Transaction { AccountId = to.AccountId, Type = "Transfer In", Amount = amount, BalanceAfter = to.Balance });
                    db.SaveChanges();
                    Console.WriteLine($"Transfer successful. New balance: {from.Balance:C}");
                }
                break;

            case "5": 
                {
                    string? accNo = ReadLineOrEsc("Enter account number: ");
                    if (accNo == null) break;
                    var account = db.Accounts.FirstOrDefault(a => a.AccountNumber == accNo && a.CustomerId == customer.CustomerId);
                    if (account == null) { Console.WriteLine("Account not found."); break; }

                    var txns = db.Transactions
                        .Where(t => t.AccountId == account.AccountId)
                        .OrderByDescending(t => t.TransactionDate)
                        .Take(5)
                        .ToList();

                    if (txns.Count == 0) { Console.WriteLine("No transactions found."); break; }
                    Console.WriteLine();
                    foreach (var t in txns)
                        Console.WriteLine($"{t.TransactionDate:dd-MM-yyyy HH:mm}  {t.Type,-12} {t.Amount,10:C}   Balance: {t.BalanceAfter:C}");
                }
                break;

            case "6": 
                {
                    string? accNo = ReadLineOrEsc("Enter account number: ");
                    if (accNo == null) break;
                    var account = db.Accounts.FirstOrDefault(a => a.AccountNumber == accNo && a.CustomerId == customer.CustomerId);
                    if (account == null) { Console.WriteLine("Account not found."); break; }

                    var request = new BankingApp.Models.ChequeBookRequest { AccountId = account.AccountId, IsApproved = false };
                    db.ChequeBookRequests.Add(request);
                    db.SaveChanges();
                    Console.WriteLine($"Cheque book requested. Your request id is {request.ChequeBookRequestId} (pending approval).");
                }
                break;

            case "7":
                {
                    string? current = ReadLineOrEsc("Enter current password: ", mask: true);
                    if (current == null) break;
                    if (!PasswordHasher.Verify(current, customer.Password)) { Console.WriteLine("Current password is incorrect."); break; }

                    string? next = ReadNewPassword("Enter new password: ");
                    if (next == null) break;
                    if (next == current) { Console.WriteLine("New password must be different from the current password."); break; }

                    string hashed = PasswordHasher.Hash(next);
                    var dbCustomer = db.Customers.First(c => c.CustomerId == customer.CustomerId);
                    dbCustomer.Password = hashed;
                    customer.Password = hashed;
                    db.SaveChanges();
                    Console.WriteLine("Password changed successfully");
                }
                break;

            case "8": 
                OpenAccount(db, customer);
                break;

            case "9":
                loggedIn = false;
                break;

            default:
                Console.WriteLine("Invalid choice");
                break;
        }

        if (loggedIn) Pause();
    }
}

// Admin Authentication 
void AdminLogin()
{
    using var db = new BankingDbContext();
    int attempts = 0;
    const int MaxAttempts = 3;

    while (true)
    {
        string? username = ReadLineOrEsc("Please enter username: ");
        if (username == null) return;   

        var admin = db.Admins.FirstOrDefault(a => a.Username == username);
        if (admin == null)
        {
            Console.WriteLine("No admin found with that username. Please try again (or press Esc to go back).");
            continue;
        }

        while (true)
        {
            string? password = ReadLineOrEsc("Please enter Password: ", mask: true);
            if (password == null) break;  

            if (PasswordHasher.Verify(password, admin.Password))
            {
                if (!PasswordHasher.IsHashed(admin.Password))
                {
                    admin.Password = PasswordHasher.Hash(password);
                    db.SaveChanges();
                }
                AdminMenu();
                return;
            }

            attempts++;
            if (attempts >= MaxAttempts)
            {
                Console.WriteLine("Too many failed attempts. Returning to the main menu.");
                Pause();
                return;
            }
            Console.WriteLine($"Incorrect password. {MaxAttempts - attempts} attempt(s) left (or press Esc to change username).");
        }
    }
}

// Admin Menu
void AdminMenu()
{
    bool loggedIn = true;
    while (loggedIn)
    {
        Header("Admin Menu");
        Console.WriteLine("1. Create New Account");
        Console.WriteLine("2. Delete Account");
        Console.WriteLine("3. Edit Account Details");
        Console.WriteLine("4. Display Summary");
        Console.WriteLine("5. Reset Customer Password");
        Console.WriteLine("6. Approve Cheque Book Request");
        Console.WriteLine("7. Exit");

        string? choice = ReadLineOrEsc("Enter your choice: ");
        if (choice == null) { loggedIn = false; break; }   

        using var db = new BankingDbContext();

        switch (choice)
        {
            case "1": 
                {
                    string? name = ReadRequired("Enter customer full name: ");
                    if (name == null) break;
                    string? username = ReadRequired("Enter a username: ");
                    if (username == null) break;
                    if (db.Customers.Any(c => c.Username == username)) { Console.WriteLine("Username already exists."); break; }

                    string? password = ReadNewPassword("Enter a password: ");
                    if (password == null) break;
                    string? email = ReadEmail("Enter email: ");
                    if (email == null) break;
                    if (db.Customers.Any(c => c.Email == email)) { Console.WriteLine("A customer with this email already exists."); break; }
                    string? phone = ReadPhone("Enter phone (10 digits): ");
                    if (phone == null) break;
                    if (db.Customers.Any(c => c.Phone == phone)) { Console.WriteLine("A customer with this phone number already exists."); break; }

                    var customer = new BankingApp.Models.Customer
                    {
                        FullName = name,
                        Username = username,
                        Password = PasswordHasher.Hash(password),
                        Email = email,
                        Phone = phone
                    };
                    db.Customers.Add(customer);
                    db.SaveChanges();

                    Console.WriteLine("Customer created. Now opening their first account.");
                    OpenAccount(db, customer);
                }
                break;

            case "2": 
                {
                    string? accNo = ReadLineOrEsc("Enter account number to delete: ");
                    if (accNo == null) break;
                    var account = db.Accounts.FirstOrDefault(a => a.AccountNumber == accNo);
                    if (account == null) { Console.WriteLine("Account not found."); break; }

                    db.Accounts.Remove(account);
                    db.SaveChanges();
                    Console.WriteLine("Account deleted successfully");
                }
                break;

            case "3": 
                {
                    string? accNo = ReadLineOrEsc("Enter account number to edit: ");
                    if (accNo == null) break;
                    var account = db.Accounts.FirstOrDefault(a => a.AccountNumber == accNo);
                    if (account == null) { Console.WriteLine("Account not found."); break; }

                    Console.WriteLine("1. Change account type");
                    Console.WriteLine("2. Activate / Deactivate");
                    string? editChoice = ReadLineOrEsc("Enter your choice: ");
                    if (editChoice == null) break;
                    if (editChoice == "1")
                    {
                        string? newType = ReadAccountType("Enter new account type (Savings/Current): ");
                        if (newType == null) break;
                        account.AccountType = newType;
                        db.SaveChanges();
                        Console.WriteLine("Account type updated.");
                    }
                    else if (editChoice == "2")
                    {
                        account.IsActive = !account.IsActive;
                        db.SaveChanges();
                        Console.WriteLine("Account is now " + (account.IsActive ? "Active" : "Inactive"));
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice");
                    }
                }
                break;

            case "4": 
                {
                    var accounts = db.Accounts.ToList();
                    Console.WriteLine();
                    Console.WriteLine("=================================");
                    Console.WriteLine("         BANK SUMMARY");
                    Console.WriteLine("=================================");
                    decimal total = 0;
                    foreach (var a in accounts)
                    {
                        total += a.Balance;
                        Console.WriteLine($"{a.AccountNumber}  {a.AccountType,-8}  {a.Balance,12:C}  {(a.IsActive ? "Active" : "Inactive")}");
                    }
                    Console.WriteLine("---------------------------------");
                    Console.WriteLine($"Total accounts : {accounts.Count}");
                    Console.WriteLine($"Total balance  : {total:C}");
                }
                break;

            case "5": 
                {
                    string? username = ReadLineOrEsc("Enter customer username: ");
                    if (username == null) break;
                    var customer = db.Customers.FirstOrDefault(c => c.Username == username);
                    if (customer == null) { Console.WriteLine("Customer not found."); break; }

                    string? next = ReadNewPassword("Enter new password: ");
                    if (next == null) break;

                    customer.Password = PasswordHasher.Hash(next);
                    db.SaveChanges();
                    Console.WriteLine("Customer password reset successfully");
                }
                break;

            case "6": 
                {
                    var pending = db.ChequeBookRequests.Where(r => !r.IsApproved).ToList();
                    if (pending.Count == 0) { Console.WriteLine("No pending requests."); break; }

                    Console.WriteLine("Pending cheque book requests:");
                    foreach (var r in pending)
                        Console.WriteLine($"Request {r.ChequeBookRequestId} - Account Id {r.AccountId}");

                    int? idInput = ReadInt("Enter request id to approve: ");
                    if (idInput == null) break;
                    int id = idInput.Value;
                    var request = db.ChequeBookRequests.FirstOrDefault(r => r.ChequeBookRequestId == id && !r.IsApproved);
                    if (request == null) { Console.WriteLine("Request not found."); break; }

                    request.IsApproved = true;
                    db.SaveChanges();
                    Console.WriteLine("Cheque book request approved successfully");
                }
                break;

            case "7":
                loggedIn = false;
                break;

            default:
                Console.WriteLine("Invalid choice");
                break;
        }

        if (loggedIn) Pause();
    }
}

// Screen helpers
void Header(string title)
{
    Console.Clear();
    Console.WriteLine("========================================");
    Console.WriteLine("   " + title);
    Console.WriteLine("========================================");
    Console.WriteLine();
}

void Pause()
{
    Console.WriteLine();
    Console.Write("Press any key to continue...");
    Console.ReadKey(intercept: true);
}

// Input helpers 

string? ReadLineOrEsc(string prompt, bool mask = false)
{
    Console.Write(prompt);
    string text = "";
    while (true)
    {
        ConsoleKeyInfo key = Console.ReadKey(intercept: true);
        if (key.Key == ConsoleKey.Escape)
        {
            Console.WriteLine();
            return null;                
        }
        if (key.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            return text;                
        }
        if (key.Key == ConsoleKey.Backspace)
        {
            if (text.Length > 0)
            {
                text = text.Substring(0, text.Length - 1);
                Console.Write("\b \b");   
            }
            continue;
        }
        if (!char.IsControl(key.KeyChar))
        {
            text += key.KeyChar;         
            Console.Write(mask ? '*' : key.KeyChar);   
        }
    }
}

string? ReadRequired(string prompt)
{
    while (true)
    {
        string? input = ReadLineOrEsc(prompt);
        if (input == null) return null;
        if (!string.IsNullOrWhiteSpace(input)) return input.Trim();
        Console.WriteLine("This field cannot be empty.");
    }
}

string? ReadNewPassword(string prompt)
{
    while (true)
    {
        string? input = ReadLineOrEsc(prompt, mask: true);
        if (input == null) return null;

        if (input.Length < 8)                  { Console.WriteLine("Password must be at least 8 characters."); continue; }
        if (!input.Any(char.IsUpper))          { Console.WriteLine("Password must contain an uppercase letter."); continue; }
        if (!input.Any(char.IsLower))          { Console.WriteLine("Password must contain a lowercase letter."); continue; }
        if (!input.Any(char.IsDigit))          { Console.WriteLine("Password must contain a number."); continue; }
        if (input.All(char.IsLetterOrDigit))   { Console.WriteLine("Password must contain a special character (e.g. @ # !)."); continue; }

        string? confirm = ReadLineOrEsc("Confirm password: ", mask: true);
        if (confirm == null) return null;
        if (confirm != input) { Console.WriteLine("Passwords do not match. Please try again."); continue; }

        return input;
    }
}

decimal? ReadAmount(string prompt)
{
    while (true)
    {
        string? input = ReadLineOrEsc(prompt);
        if (input == null) return null;
        if (decimal.TryParse(input, out decimal amount) && amount > 0)
            return amount;
        Console.WriteLine("Please enter a valid positive number.");
    }
}

int? ReadInt(string prompt)
{
    while (true)
    {
        string? input = ReadLineOrEsc(prompt);
        if (input == null) return null;
        if (int.TryParse(input, out int value))
            return value;
        Console.WriteLine("Please enter a valid whole number.");
    }
}

string? ReadPhone(string prompt)
{
    while (true)
    {
        string? input = ReadLineOrEsc(prompt);
        if (input == null) return null;
        if (Regex.IsMatch(input, @"^\d{10}$"))
            return input;
        Console.WriteLine("Phone must be exactly 10 digits.");
    }
}

string? ReadEmail(string prompt)
{
    while (true)
    {
        string? input = ReadLineOrEsc(prompt);
        if (input == null) return null;
        if (Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return input;
        Console.WriteLine("Please enter a valid email (e.g. name@email.com).");
    }
}

string? ReadAccountType(string prompt)
{
    while (true)
    {
        string? input = ReadLineOrEsc(prompt);
        if (input == null) return null;
        string t = input.Trim().ToLower();
        if (t == "savings") return "Savings";
        if (t == "current") return "Current";
        Console.WriteLine("Account type must be Savings or Current.");
    }
}