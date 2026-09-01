Console.WriteLine("Hello, World!");

#region notes
// string is a keyword
// String is a class

// There are 2 types of data types in C#
// 1. Primitive data types - int, double, bool, char - the 1 provided by C# language
// 2. (UDT)Non-primitive data types - string, object, array - the 1 provided by .NET framework
// 3. User-defined data types(by developers) - classes, interfaces, delegates

// There are 2 ways to look at data types in C#
// 1. Value types - int, double, bool, char, struct, enum
// 2. Reference types - string, object, array
#endregion

#region Input

System.Console.WriteLine("!~~~~~~~~~~~~~~ Welcome to CITI Bank ~~~~~~~~~~~~~~!");
string name = string.Empty;
System.Console.WriteLine("Please enter your name: ");
name = System.Console.ReadLine();

string city = string.Empty;
System.Console.WriteLine("Please enter your city: ");
city = System.Console.ReadLine();

int age = 0;
System.Console.WriteLine("Please enter your age: ");
age = Convert.ToInt32(System.Console.ReadLine());

bool isMarried;
System.Console.WriteLine("Are you married? (true/false): ");
isMarried = Convert.ToBoolean(System.Console.ReadLine());

Console.WriteLine("Thank you for providing your details. We will process your information and get back to you shortly.");

#region  Conditional Processing of values

bool isNameValid = true;
bool isCityValid = true;
bool isAgeValid = true;
bool isMaritalStatusValid = true; // bool input itself is always valid, but kept for consistency

// Validate Name
if (string.IsNullOrEmpty(name) || name.Length < 3 || name.Length > 25)
{
    isNameValid = false;
    System.Console.WriteLine("Invalid name. Please enter a valid name with 3 to 25 characters.");
}
else
{
    // Convert the first letter to uppercase and the rest to lowercase
    name = name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();
}

// Validate City
if (string.IsNullOrEmpty(city) ||
    (city != "New York" && city != "Los Angeles" && city != "Chicago"))
{
    isCityValid = false;
    System.Console.WriteLine("Invalid city. Please enter a valid city: New York, Los Angeles, or Chicago.");
}

// Validate Age
if (age <= 0)
{
    isAgeValid = false;
    System.Console.WriteLine("Invalid age. Age cannot be zero or negative.");
}
else if (age < 18 || age > 60)
{
    isAgeValid = false;
    System.Console.WriteLine("Invalid age. Please enter an age between 18 and 60.");
}

// Validate Marital Status
// isMarried is already guaranteed true/false by Convert.ToBoolean, so no separate check needed.

bool validationsPassed = isNameValid && isCityValid && isAgeValid && isMaritalStatusValid;

if (validationsPassed)
{
    System.Console.WriteLine(
        $"Approved!! Thank you {name} from {city}, age {age}, for providing your details."
    );
}
else
{
    System.Console.WriteLine(
        $"Rejected!! {name}, please correct the errors and try again."
    );
}

#endregion

#endregion

string empFirstName = "kamya";
string designation = "Software Engineer";
int empAge = 25;
double salary = 100000.00;
double height = 5.6;
bool empIsMarried = false;

System.Console.WriteLine("First Name: " + empFirstName + "\nDesignation: " + designation + "\nAge: " + empAge + "\nSalary: " + salary + "\nHeight: " + height + "\nIs Married: " + empIsMarried);