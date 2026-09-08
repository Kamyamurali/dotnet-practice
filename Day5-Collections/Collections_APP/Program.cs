using System.Collections; 
using EmployeeManagement;

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

#region List
// List<string> friends = new List<string>();
// friends.Add("Alice");
// string newfriend = "a";
// while (newfriend != "")
// {
//     Console.WriteLine("Please enter your friend name");
//     newfriend = Console.ReadLine();
//     friends.Add(newfriend);
    
// }
// Console.WriteLine("The total number of friends in the list is: " + friends.Count);
#endregion

List<Employee> employees = new List<Employee>();
employees.Add(new Employee(1, "John Doe", "IT", 50000));
employees.Add(new Employee(2, "Jane Smith", "HR", 60000));
employees.Add(new Employee(3, "Mike Johnson", "Finance", 55000));
employees.Add(new Employee(4, "Emily Davis", "IT", 70000));
employees.Add(new Employee(5, "David Wilson", "HR", 65000));
employees.Add(new Employee(6, "Sarah Brown", "Finance", 60000));
employees.Add(new Employee(7, "Chris Lee", "IT", 75000));
employees.Add(new Employee(8, "Amy Taylor", "HR", 62000));
employees.Add(new Employee(9, "James Anderson", "Finance", 58000));
employees.Add(new Employee(10, "Olivia Martinez", "IT", 72000));
employees.Add(new Employee(11, "Daniel Thomas", "HR", 64000));

int count_isPermanent = 0;
int count_notPermanent = 0;
int highestSalary = 0;
for (var employee in employees)
{
    if (employee.empIsPermanent)
    {
        count_isPermanent++;
    }
    else
    {
        count_notPermanent++;
    }

    if (employee.Salary > highestSalary)
    {
        highestSalary = (int)employee.Salary;
    }
}

for each (var employee in employees)
{
Console.WriteLine($"ID: {employee.Id}, Name: {employee.Name}, Department: {employee.Department}, Salary: {employee.Salary}");
}
Console.WriteLine($"Total Permanent Employees: {count_isPermanent}");
Console.WriteLine($"Total Non-Permanent Employees: {count_notPermanent}");
Console.WriteLine($"Highest Salary: {highestSalary}");