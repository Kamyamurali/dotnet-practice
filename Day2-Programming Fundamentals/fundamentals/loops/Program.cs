//for loop
// for (int i = 0; i < 10; i++)
// {
//     Console.WriteLine(i);
// }
// string[] names = { "John", "Jane", "Jack", "Jill" };
// Console.WriteLine(names[3]);

#region guess the number

int secretNumber = 7;

Console.WriteLine("Guess the number, you have 3 attempts!");

int attempts = 0;

for (int i = 0; i < 3; i++)
{
    Console.WriteLine("Enter your guess: ");

    int userGuess = Convert.ToInt32(Console.ReadLine());

    attempts++;

    if (userGuess == secretNumber)
    {
        Console.WriteLine($"Congratulations! You guessed the number in {attempts} attempts.");
        break;
    }
    else
    {
        Console.WriteLine("Wrong guess, try again.");
    }
}

#endregion



// Our requirement is to ask the user to enter a number
// and keep entering until the number is 0

int userInput = 0;
int addition = 0;
int evenNumber = 0;
int oddNumber = 0;
int totalNumbers = 0;
int greaterThan100 = 0;

do
{
    Console.WriteLine("Enter a number (enter 0 to exit): ");
    userInput = Convert.ToInt32(Console.ReadLine());
    if (userInput != 0)
    {
        Console.WriteLine($"You entered: {userInput}");
        addition += userInput; //addition = addition + userInput
        totalNumbers++;

        if (userInput % 2 == 0)
        {
            evenNumber++;
        }
        else
        {
            oddNumber++;
        }

        if (userInput > 100)
        {
            greaterThan100++;
        }
    }
} while (userInput != 0);

Console.WriteLine($"Total numbers entered: {totalNumbers}");
Console.WriteLine("Addition : " + addition);
Console.WriteLine($"Sum of all numbers: {addition}");   
Console.WriteLine($"Total even numbers: {evenNumber}");
Console.WriteLine($"Total odd numbers: {oddNumber}");
Console.WriteLine($"Total numbers greater than 100: {greaterThan100}");