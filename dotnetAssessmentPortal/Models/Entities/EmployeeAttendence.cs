namespace dotnetAssessmentPortal.Models.Entities
{
    public class EmployeeAttendence
    {
		public int Id { get; set; }
		public int EmployeeId { get; set; }
		public DateTime AttendanceDate { get; set; }
		public bool IsPresent { get; set; }
		public bool IsAbsent { get; set; }
		public bool IsOffday { get; set; }
	}
}
