using employeeManagement;

Employee emp1 = new Employee()
{
    EmpId = 1,
    EmpName = "Kamya",
    Department = "Engineering",
    Salary = 50000,
    IsPresent = true
};

Console.WriteLine("--- Employee Details ---");
emp1.DisplayDetails();

Console.WriteLine();
Console.WriteLine("--- Giving a raise ---");
emp1.GiveRaise(10);

Console.WriteLine();
Console.WriteLine("--- Updated Details ---");
emp1.DisplayDetails();