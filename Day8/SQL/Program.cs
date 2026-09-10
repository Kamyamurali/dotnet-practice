using EmpManagement;

Employee empObj = new Employee()
{
    empNo = 101,
    empName = "John Doe",
    empSalary = 50000,
    empIsActive = true,
    empPassword = "password123", // This property will be ignored during serialization

};

Console.WriteLine("Select from options:");
Console.WriteLine("1. Appraise Salary");
Console.WriteLine("2. Apply Leave");
Console.WriteLine("3. Change first name ");
Console.WriteLine("4. Exit");

bool Continuework = true;
while (Continuework)
{
    int choice = Convert.ToInt32(Console.ReadLine());
    switch (choice)
    {
        case 1:
            double newSalary = empObj.AppraiseSalary();
            Console.WriteLine($"New Salary after appraisal: {newSalary}");
            break;
        case 2:
            Console.WriteLine("Enter number of leave days to apply:");
            int leaveDays = Convert.ToInt32(Console.ReadLine());
            try
            {
                int remainingLeave = empObj.ApplyLeave(leaveDays);
                Console.WriteLine($"Leave applied successfully. Remaining leave: {remainingLeave}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            break;
        case 3:
            Console.WriteLine("Enter new first name:");
            string newFirstName = Console.ReadLine();
            empObj.empName = newFirstName;
            Console.WriteLine($"First name changed to: {empObj.empName}");
            break;
                    case 4:
            Continuework = false;
            break;
        default:
            Console.WriteLine("Invalid choice. Please select a valid option.");
            break;
    }
}


       

