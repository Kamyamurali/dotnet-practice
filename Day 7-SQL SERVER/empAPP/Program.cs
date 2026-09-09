using empAPP.DB;

EmployeeManagementDbContext db = new EmployeeManagementDbContext();

// Count total departments
var totalDept = (from e in db.Depts
                 select e.DeptNo).Count();
Console.WriteLine("Total Departments : " + totalDept);

// List all employees, ordered by designation
var allEmps = from e in db.Employees
              orderby e.EmpDesignation
              select e;

foreach (var item in allEmps)
{
    Console.WriteLine(item.EmpNo + " " + item.EmpName + " " + item.EmpDesignation);
}

Console.WriteLine("Enter empNo to search employee details :");  
int empNo = Convert.ToInt32(Console.ReadLine());
var emp = (from e in db.Employees
           where e.EmpNo == empNo
           select e).Single();

Console.WriteLine(emp.EmpNo);
Console.WriteLine(emp.EmpName); 
Console.WriteLine(emp.EmpDesignation);
Console.WriteLine(emp.EmpSalary);

//delete employee
Console.WriteLine("Enter empNo to delete employee details :");
int empNoToDelete = Convert.ToInt32(Console.ReadLine());
var empToDelete = (from e in db.Employees
                   where e.EmpNo == empNoToDelete
                   select e).Single();
db.Employees.Remove(empToDelete);
int result = db.SaveChanges();  

