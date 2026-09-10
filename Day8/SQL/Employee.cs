using System.Text.Json;
using System.Text.Json.Serialization;

namespace EmpManagement
{
    public class Employee
    {
        [JsonRequired]
        public int empNo { get; set; }
        public string empName { get; set; }
        [JsonPropertyName("Salary")]

        public double empSalary { get; set; }
        [JsonPropertyName("Permanent")]
        public bool empIsActive { get; set; }
        public int empAvailableLeave{ get; set;}
        [JsonIgnore]
        public string empPassword { get; set; } //cant get saved to a file 
         
        public double AppraiseSalary()
        {
            empSalary += 2000;
            return empSalary;
        }
        public int ApplyLeave(int days)
        {
            if (days > 5)
            {
                throw new Exception("You cannot apply for more than 5 days leave at a time.");
            }
            empAvailableLeave -= days;
            return empAvailableLeave;
        }
        public string SaveObjectToJson()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
            JsonSerializer.Serialize(this, options);
            return "Object saved to JSON successfully.";
        }
        
    }
}


     