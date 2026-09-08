using System.Collections; 

#region Array
// int[] myNumber = new int[10];
// for (int i = 0; i < myNumber.Length; i++)
// {
//     Console.WriteLine("Please enter your " + i + " Number");
//     myNumber[i] = Convert.ToInt32(Console.ReadLine());
// }

// int additions = 0;
// int evenNumber = 0;
// int oddNumber = 0;

// for (int i = 0; i < myNumber.Length; i++)
// {
//     additions = additions + myNumber[i];
//     if(myNumber[i] % 2 == 0)
//     {
//         evenNumber++;
//     }
//     else
//     {
//         oddNumber++;
//     }
// }
// Console.WriteLine("The total of the numbers is: " + additions);
// Console.WriteLine("The total of even numbers is: " + evenNumber);
// Console.WriteLine("The total of odd numbers is: " + oddNumber);
#endregion


#region  ArrayList
// ArrayList myList = new ArrayList();
// myList.Add(10);
// myList.Add("kamya");
// myList.Add(20.5);
// myList.Add(true);
// mylist.Add(new DateTime(2024, 6, 1));
// myList.Add(new int[] { 1, 2, 3, 4, 5 });
// myList.Add(new{employeeId = 1, employeeName = "John Doe", employeeSalary = 50000});

// foreach (var item in myList)
// {
//     Console.WriteLine(item);
// }
// console.WriteLine("The total number of items in the list is: " + myList.Count);
#endregion

List<string> friends = new List<string>();
friends.Add("Alice");
string newfriend = "a";
while (newfriend != "")
{
    Console.WriteLine("Please enter your friend name");
    newfriend = Console.ReadLine();
    friends.Add(newfriend);
    
}
Console.WriteLine("The total number of friends in the list is: " + friends.Count);