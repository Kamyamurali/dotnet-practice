namespace employeeManagement
{
    class Employee
    {
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string Department { get; set; }
        public double Salary { get; set; }
        public bool IsPresent { get; set; }

        public void GiveRaise(double percent)
        {
            double increase = Salary * percent / 100;
            Salary = Salary + increase;
            Console.WriteLine(EmpName + " got a " + percent + "% raise. New salary: " + Salary);
        }

        public void DisplayDetails()
        {
            Console.WriteLine("Employee ID : " + EmpId);
            Console.WriteLine("Name        : " + EmpName);
            Console.WriteLine("Department  : " + Department);
            Console.WriteLine("Salary      : " + Salary);
            Console.WriteLine("Present     : " + IsPresent);
        }
    }
}