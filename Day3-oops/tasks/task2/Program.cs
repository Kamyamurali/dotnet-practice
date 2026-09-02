using System;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        Console.WriteLine("Welcome to guest management system");
        Console.WriteLine("a. New Guest");
        Console.WriteLine("b. View Guest Details");
        Console.Write("Choose an option: ");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "a":
                NewGuest();
                break;
            case "b":
                ViewGuest();
                break;
            default:
                Console.WriteLine("Invalid option");
                break;
        }
    }

    static void NewGuest()
    {
        Console.Write("SSN Number : ");
        string ssn = Console.ReadLine();

        Console.Write("First Name : ");
        string firstName = Console.ReadLine();

        Console.Write("Last Name  : ");
        string lastName = Console.ReadLine();

        Console.Write("Email      : ");
        string email = Console.ReadLine();

        Console.Write("Phone      : ");
        string phone = Console.ReadLine();

        Console.WriteLine("Notes (press Enter on a blank line to finish):");
        StringBuilder notes = new StringBuilder();
        while (true)
        {
            string line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                break;
            }
            notes.AppendLine(line);
        }

        StringBuilder content = new StringBuilder();
        content.AppendLine("SSNO       : " + ssn);
        content.AppendLine("First Name : " + firstName);
        content.AppendLine("Last Name  : " + lastName);
        content.AppendLine("Email      : " + email);
        content.AppendLine("Phone      : " + phone);
        content.AppendLine("Notes      : " + notes.ToString());

        string fileName = ssn + ".txt";          
        File.WriteAllText(fileName, content.ToString());

        Console.WriteLine("Guest saved to " + fileName);
    }

    static void ViewGuest()
    {
        Console.Write("Please enter SSN No : ");
        string ssn = Console.ReadLine();

        string fileName = ssn + ".txt";

        if (File.Exists(fileName))
        {
            Console.WriteLine();
            Console.WriteLine(File.ReadAllText(fileName));
        }
        else
        {
            Console.WriteLine("No guest found with SSN " + ssn);
        }
    }
}