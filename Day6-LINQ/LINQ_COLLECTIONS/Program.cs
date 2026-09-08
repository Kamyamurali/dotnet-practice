using System.Runtime.CompilerServices;
using EmployeeManagement;
List<Employee> eList = new List<Employee>()
#region Employee Data
{
    new Employee(){ empNo = 1, empName = "John Doe", empDepartmentNo = 101, empIsPermanent = true, empSalary = 50000 },
    new Employee(){ empNo = 2, empName = "Jane Smith", empDepartmentNo = 102, empIsPermanent = false, empSalary = 45000 },
    new Employee(){ empNo = 3, empName = "Michael Johnson", empDepartmentNo = 101, empIsPermanent = true, empSalary = 60000 },
    new Employee(){ empNo = 4, empName = "Emily Davis", empDepartmentNo = 103, empIsPermanent = false, empSalary = 40000 },
    new Employee(){ empNo = 5, empName = "William Brown", empDepartmentNo = 102, empIsPermanent = true, empSalary = 55000 },
    new Employee(){ empNo = 6, empName = "Olivia Wilson", empDepartmentNo = 101, empIsPermanent = true, empSalary = 70000 },
    new Employee(){ empNo = 7, empName = "James Taylor", empDepartmentNo = 103, empIsPermanent = false, empSalary = 48000 },
    new Employee(){ empNo = 8, empName = "Sophia Anderson", empDepartmentNo = 102, empIsPermanent = true, empSalary = 52000 },
    new Employee(){ empNo = 9, empName = "Benjamin Thomas", empDepartmentNo = 101, empIsPermanent = false, empSalary = 43000 },
    new Employee(){ empNo = 10, empName = "Ava Martinez", empDepartmentNo = 103, empIsPermanent = true, empSalary = 58000 },
    new Employee(){ empNo = 11, empName = "Liam Garcia", empDepartmentNo = 102, empIsPermanent = true, empSalary = 62000 },
    new Employee(){ empNo = 12, empName = "Mia Rodriguez", empDepartmentNo = 101, empIsPermanent = false, empSalary = 47000 },
    new Employee(){ empNo = 13, empName = "Noah Lee", empDepartmentNo = 103, empIsPermanent = true, empSalary = 54000 },
    new Employee(){ empNo = 14, empName = "Isabella Perez", empDepartmentNo = 102, empIsPermanent = false, empSalary = 46000 },
    new Employee(){ empNo = 15, empName = "Elijah White", empDepartmentNo = 101, empIsPermanent = true, empSalary = 59000 },
    new Employee(){ empNo = 16, empName = "Charlotte Harris", empDepartmentNo = 103, empIsPermanent = false, empSalary = 42000 },
    new Employee(){ empNo = 17, empName = "Alexander Clark", empDepartmentNo = 102, empIsPermanent = true, empSalary = 61000 },
    new Employee(){ empNo = 18, empName = "Amelia Lewis", empDepartmentNo = 101, empIsPermanent = false, empSalary = 44000 },
    new Employee(){ empNo = 19, empName = "Daniel Robinson", empDepartmentNo = 103, empIsPermanent = true, empSalary = 57000 },
    new Employee(){ empNo = 20, empName = "Harper Walker", empDepartmentNo = 102, empIsPermanent = false, empSalary = 49000 }

};
#endregion

//this is select * from source
var emp = from e in eList
//sort syntax
//filter syntax
//groupby syntax
//calculation 
//conditions
             select e;
        foreach (var e in emp)
        {
            Console.WriteLine($"Employee No: {e.empNo}, Name: {e.empName}, Department No: {e.empDepartmentNo}, Is Permanent: {e.empIsPermanent}, Salary: {e.empSalary}");

        }
//  emp = from e in eList
//           where e.empNo > 10
//           select e;
// foreach (var e in emp)
// {
//     Console.WriteLine($"Employee No: {e.empNo}, Name: {e.empName}, Department No: {e.empDepartmentNo}, Is Permanent: {e.empIsPermanent}, Salary: {e.empSalary}");
// }
  
//  emp = from e in eList
//           where e.empIsPermanent == true
//           select e;
// foreach (var e in emp)
// {
//     Console.WriteLine($"Employee No: {e.empNo}, Name: {e.empName}, Department No: {e.empDepartmentNo}, Is Permanent: {e.empIsPermanent}, Salary: {e.empSalary}");
// }
//  emp = from e in eList
//           where e.empDepartmentNo == 101 && e.empSalary > 50000
//           select e;
// foreach (var e in emp)
// {
//     Console.WriteLine($"Employee No: {e.empNo}, Department No: {e.empDepartmentNo}, Salary: {e.empSalary}");
// }
//  emp = from e in eList
//           orderby e.empSalary descending
//           select e;
// foreach (var e in emp)
// {
//     Console.WriteLine($"Employee No: {e.empNo}, Name: {e.empName}");
// }
//  var totalemp = (from e in eList
//                 where e.empIsPermanent == true && e.empDepartmentNo == 101
//                 select e.empNo).Count();

// Console.WriteLine("Total Employees are : " + totalemp);

// var minSalary = (from e in eList
//                  where e.empIsPermanent == true && e.empDepartmentNo == 101
//                  select e.empSalary).Min();
// Console.WriteLine("Minimum Salary is : " + minSalary);

// var maxSalary = (from e in eList
//                  where e.empIsPermanent == true && e.empDepartmentNo == 101
//                  select e.empSalary).Max();
// Console.WriteLine("Maximum Salary is : " + maxSalary);

// var totalempindept = (from e in eList
//                      where e.empIsPermanent == true && e.empDepartmentNo == 101
//                      select e.empSalary).Sum();
// Console.WriteLine("Total Salary in Department 101 is : " + totalempindept);
 #region Selecting Employees based on conditions
//employees having ar in their name
var empwithar = from e in eList
                where e.empName.Contains("ar")
                select e;
foreach (var e in empwithar)
{
    Console.WriteLine($"Employee No: {e.empNo}, Name: {e.empName}, Department No: {e.empDepartmentNo}, Is Permanent: {e.empIsPermanent}, Salary: {e.empSalary}");
}

//employees having name second letter as 'a'
var empwithsecondlettera = from e in eList
                           where e.empName.Length > 1 && e.empName[1] == 'a'
                           select e;
foreach (var e in empwithsecondlettera)
{
    Console.WriteLine($"Employee No: {e.empNo}, Name: {e.empName}, Department No: {e.empDepartmentNo}, Is Permanent: {e.empIsPermanent}, Salary: {e.empSalary}");
}
#endregion

#region  Calculations
var totalSalary = (from e in eList
                   where e.empIsPermanent == true
                   select e.empSalary).Sum();
Console.WriteLine("Total Salary of Permanent Employees: " + totalSalary);       

var averageSalary = (from e in eList
                     where e.empIsPermanent == true
                     select e.empSalary).Average();
Console.WriteLine("Average Salary of Permanent Employees: " + averageSalary);   

var employmentsummary = from e in eList
                    group e by e.empDepartmentNo into deptGroup
                    select new
                    {
                        DepartmentNo = deptGroup.Key,
                        TotalEmployees = deptGroup.Count(),
                        AverageSalary = deptGroup.Average(emp => emp.empSalary)
                    };
                    foreach (var summary in employmentsummary)
                    {
                        Console.WriteLine($"Department No: {summary.DepartmentNo}, Total Employees: {summary.TotalEmployees}, Average Salary: {summary.AverageSalary}");
                    }
#endregion

//lamda
var totalSalaryLambda = eList.Sum(e => e.empSalary);
Console.WriteLine("Total Salary of Permanent Employees (Lambda): " + totalSalaryLambda);