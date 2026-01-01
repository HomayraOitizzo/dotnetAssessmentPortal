using dotnetAssessmentPortal.Models.DTOs;
using dotnetAssessmentPortal.Models.Entities;

namespace dotnetAssessmentPortal.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(int employeeId);
        Task<bool> IsCodeAssignedToOtherEmployeeAsync(string employeeCode, int excludeEmployeeId);
        Task<bool> UpdateEmployeeAsync(Employee employee);
        Task<bool> SaveChangesAsync();
        Task<Employee?> GetNthHighestSalaryEmployeeAsync(int rank);
        Task<List<Employee>> GetHighEarnersWithPerfectAttendanceAsync(decimal minSalary);
        Task<List<MonthlyAttendanceReportDto>> GetMonthlyAttendanceReportAsync(int month, int year);
        Task<List<string>> GetSupervisorHierarchyAsync(int employeeId);
    }
}

