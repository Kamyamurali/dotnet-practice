using System;
using System.IO;

class Program
{
    static void Main()
    {
        string filePath = "counter.txt";
        int counter;

        // First run ever: file doesn't exist yet, so start from 0
        if (File.Exists(filePath))
        {
            string text = File.ReadAllText(filePath);
            counter = int.Parse(text);
        }
        else
        {
            counter = 0;
        }

        counter++;                                        
        File.WriteAllText(filePath, counter.ToString());  

        for (int i = 1; i <= counter; i++)
        {
            Console.Write(i);
            if (i < counter)
            {
                Console.Write(", ");
            }
        }
        Console.WriteLine();
    }
}
