namespace dotnetAssessmentPortal.Models.Entities
{
    public class Employee
    {
		public int EmployeeId { get; set; }
		public required string EmployeeName { get; set; }
		public required string EmployeeCode { get; set; }
		public decimal EmployeeSalary { get; set; }
		public int? SupervisorId { get; set; }

	 
	}
}
