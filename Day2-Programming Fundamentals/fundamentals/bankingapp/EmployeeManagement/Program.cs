Console.WriteLine("Welcome to Employee Management System");

List<Employee> employees = new List<Employee>();
List<string> leaveRequests = new List<string>();
List<string> reimbursements = new List<string>();
List<string> activityAnnouncements = new List<string>();
List<string> openPositions = new List<string> { "Software Engineer", "Business Analyst", "QA Tester" };

string adminUsername = "revadmin";
string adminPassword = "revadmin$123#";
string employeeUsername = "revemp2409";
string employeePassword = "revadmin$123#emp";

bool exitProgram = false;

while (!exitProgram)
{
    Console.Clear();
    Console.WriteLine("Welcome to Employee Management System");
    Console.WriteLine();
    Console.WriteLine("Please select an option:");
    Console.WriteLine("a. Admin");
    Console.WriteLine("b. Employee");
    Console.WriteLine("c. Guest");
    Console.WriteLine("g. Exit");
    Console.Write("Enter your choice: ");

    string mainChoice = Console.ReadLine();

    switch (mainChoice)
    {
        case "a":
            Console.Write("Enter username: ");
            string adminUserInput = Console.ReadLine();
            Console.Write("Enter password: ");
            string adminPassInput = Console.ReadLine();

            if (adminUserInput != adminUsername || adminPassInput != adminPassword)
            {
                Console.WriteLine("Invalid credentials.");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
                break;
            }

            bool backToMain1 = false;
            while (!backToMain1)
            {
                Console.Clear();
                Console.WriteLine("=== Admin Menu ===");
                Console.WriteLine("a. Create new employee");
                Console.WriteLine("b. Change employee details");
                Console.WriteLine("c. Announce Activity");
                Console.WriteLine("d. Delete Employee");
                Console.WriteLine("e. View All employees");
                Console.WriteLine("f. Back to previous menu");
                Console.WriteLine("g. Exit");
                Console.Write("Enter your choice: ");

                string adminChoice = Console.ReadLine();

                switch (adminChoice)
                {
                    case "a":
                        Console.Write("Enter employee name: ");
                        string newName = Console.ReadLine();
                        Console.Write("Enter role: ");
                        string newRole = Console.ReadLine();
                        Console.Write("Enter department: ");
                        string newDept = Console.ReadLine();

                        Employee newEmployee = new Employee();
                        newEmployee.Name = newName;
                        newEmployee.Role = newRole;
                        newEmployee.Department = newDept;
                        employees.Add(newEmployee);

                        Console.WriteLine(newName + " has been added as a new employee.");
                        break;

                    case "b":
                        Console.Write("Enter employee name to update: ");
                        string nameToUpdate = Console.ReadLine();
                        Employee foundEmployee = null;

                        foreach (Employee emp in employees)
                        {
                            if (emp.Name == nameToUpdate)
                            {
                                foundEmployee = emp;
                            }
                        }

                        if (foundEmployee == null)
                        {
                            Console.WriteLine("Employee not found.");
                        }
                        else
                        {
                            Console.Write("Enter new role: ");
                            foundEmployee.Role = Console.ReadLine();
                            Console.Write("Enter new department: ");
                            foundEmployee.Department = Console.ReadLine();
                            Console.WriteLine("Employee details updated.");
                        }
                        break;

                    case "c":
                        Console.Write("Enter announcement: ");
                        string announcement = Console.ReadLine();
                        activityAnnouncements.Add(announcement);
                        Console.WriteLine("Announcement posted.");
                        break;

                    case "d":
                        Console.Write("Enter employee name to delete: ");
                        string nameToDelete = Console.ReadLine();
                        Employee employeeToRemove = null;

                        foreach (Employee emp in employees)
                        {
                            if (emp.Name == nameToDelete)
                            {
                                employeeToRemove = emp;
                            }
                        }

                        if (employeeToRemove == null)
                        {
                            Console.WriteLine("Employee not found.");
                        }
                        else
                        {
                            employees.Remove(employeeToRemove);
                            Console.WriteLine(nameToDelete + " has been deleted.");
                        }
                        break;

                    case "e":
                        Console.WriteLine("=== All Employees ===");
                        if (employees.Count == 0)
                        {
                            Console.WriteLine("No employees found.");
                        }
                        else
                        {
                            foreach (Employee emp in employees)
                            {
                                Console.WriteLine("- " + emp.Name + " | " + emp.Role + " | " + emp.Department);
                            }
                        }
                        break;

                    case "f":
                        backToMain1 = true;
                        break;

                    case "g":
                        backToMain1 = true;
                        exitProgram = true;
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                if (!backToMain1)
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
            break;

        case "b":
            Console.Write("Enter username: ");
            string employeeUserInput = Console.ReadLine();
            Console.Write("Enter password: ");
            string employeePassInput = Console.ReadLine();

            if (employeeUserInput != employeeUsername || employeePassInput != employeePassword)
            {
                Console.WriteLine("Invalid credentials.");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
                break;
            }

            bool backToMain2 = false;
            while (!backToMain2)
            {
                Console.Clear();
                Console.WriteLine("=== Employee Menu ===");
                Console.WriteLine("a. View my details");
                Console.WriteLine("b. Apply leave");
                Console.WriteLine("c. Submit reimbursement");
                Console.WriteLine("d. View project details");
                Console.WriteLine("e. View todays task and activities");
                Console.WriteLine("f. Previous menu");
                Console.WriteLine("g. Exit");
                Console.Write("Enter your choice: ");

                string employeeChoice = Console.ReadLine();

                switch (employeeChoice)
                {
                    case "a":
                        Console.Write("Enter your name: ");
                        string myName = Console.ReadLine();
                        Employee me = null;

                        foreach (Employee emp in employees)
                        {
                            if (emp.Name == myName)
                            {
                                me = emp;
                            }
                        }

                        if (me == null)
                        {
                            Console.WriteLine("No record found. Ask Admin to add you first.");
                        }
                        else
                        {
                            Console.WriteLine("Name: " + me.Name);
                            Console.WriteLine("Role: " + me.Role);
                            Console.WriteLine("Department: " + me.Department);
                        }
                        break;

                    case "b":
                        Console.Write("Enter your name: ");
                        string leaveName = Console.ReadLine();
                        Console.Write("Enter leave reason: ");
                        string leaveReason = Console.ReadLine();
                        leaveRequests.Add(leaveName + " - " + leaveReason);
                        Console.WriteLine("Leave request submitted.");
                        break;

                    case "c":
                        Console.Write("Enter your name: ");
                        string reimburseName = Console.ReadLine();
                        Console.Write("Enter reimbursement amount: ");
                        string reimburseAmount = Console.ReadLine();
                        reimbursements.Add(reimburseName + " - $" + reimburseAmount);
                        Console.WriteLine("Reimbursement submitted.");
                        break;

                    case "d":
                        Console.WriteLine("=== Project Details ===");
                        Console.WriteLine("Project: Employee Management System");
                        Console.WriteLine("Status: In Progress");
                        Console.WriteLine("Deadline: End of Sprint");
                        break;

                    case "e":
                        Console.WriteLine("=== Today's Tasks ===");
                        if (activityAnnouncements.Count == 0)
                        {
                            Console.WriteLine("No announcements or tasks yet.");
                        }
                        else
                        {
                            foreach (string activity in activityAnnouncements)
                            {
                                Console.WriteLine("- " + activity);
                            }
                        }
                        break;

                    case "f":
                        backToMain2 = true;
                        break;

                    case "g":
                        backToMain2 = true;
                        exitProgram = true;
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                if (!backToMain2)
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
            break;

        case "c":
            Console.Clear();
            Console.WriteLine("=== Guest Menu ===");
            Console.WriteLine("a. About the organization");
            Console.WriteLine("b. View Open Positions");
            Console.WriteLine("c. Contact information");
            Console.Write("Enter your choice: ");

            string guestChoice = Console.ReadLine();

            switch (guestChoice)
            {
                case "a":
                    Console.WriteLine("We are Particle Black, a company focused on business analysis and technology consulting.");
                    break;

                case "b":
                    Console.WriteLine("=== Open Positions ===");
                    foreach (string position in openPositions)
                    {
                        Console.WriteLine("- " + position);
                    }
                    break;

                case "c":
                    Console.WriteLine("Email: contact@company.com");
                    Console.WriteLine("Phone: 123-456-7890");
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            break;

        case "g":
            Console.WriteLine("Goodbye!");
            exitProgram = true;
            break;

        default:
            Console.WriteLine("Invalid choice. Try again.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            break;
    }
}

class Employee
{
    public string Name;
    public string Role;
    public string Department;
}