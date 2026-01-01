namespace dotnetAssessmentPortal.Models.DTOs
{
    public class MonthlyAttendanceReportDto
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string MonthName { get; set; } = string.Empty;
        public decimal CalculatedPayableSalary { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int OffdaysCount { get; set; }
    }
}

