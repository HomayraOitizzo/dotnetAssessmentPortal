using dotnetAssessmentPortal.Data;
using dotnetAssessmentPortal.Models.DTOs;
using dotnetAssessmentPortal.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace dotnetAssessmentPortal.Repositories
{
	public class EmployeeRepository : IEmployeeRepository
	{
		private readonly ApplicationDbContext _context;

		public EmployeeRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Employee?> GetByIdAsync(int employeeId)
		{
			return await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
		}

		public async Task<bool> IsCodeAssignedToOtherEmployeeAsync(string employeeCode, int excludeEmployeeId)
		{
			return await _context.Employees.AnyAsync(e => e.EmployeeCode == employeeCode && e.EmployeeId != excludeEmployeeId);
		}

		public async Task<bool> UpdateEmployeeAsync(Employee employee)
		{
			_context.Employees.Update(employee);
			return await SaveChangesAsync();
		}

		public async Task<bool> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<Employee?> GetNthHighestSalaryEmployeeAsync(int rank)
		{
			return await _context.Employees.OrderByDescending(e => e.EmployeeSalary)
				.Skip(rank - 1)
				.Take(1)
				.FirstOrDefaultAsync();
		}

		public async Task<List<Employee>> GetHighEarnersWithPerfectAttendanceAsync(decimal minSalary)
		{
			return await _context.Employees.Where(e => e.EmployeeSalary >= minSalary)
				.Where(e => !_context.EmployeeAttendences
					.Any(a => a.EmployeeId == e.EmployeeId && a.IsAbsent))
				.OrderByDescending(e => e.EmployeeSalary)
				.ToListAsync();
		}

		public async Task<List<MonthlyAttendanceReportDto>> GetMonthlyAttendanceReportAsync(int month, int year)
		{
			DateTime startDate = new DateTime(year, month, 1);
			DateTime endDate = startDate.AddMonths(1).AddDays(-1);
			string monthName = startDate.ToString("MMMM");

			var allEmployees = await _context.Employees.ToListAsync();

			List<MonthlyAttendanceReportDto> report = new List<MonthlyAttendanceReportDto>();

			foreach (var employee in allEmployees)
			{

				int presentCount = await _context.EmployeeAttendences
					.CountAsync(a => a.EmployeeId == employee.EmployeeId &&
								   a.AttendanceDate >= startDate &&
								   a.AttendanceDate <= endDate &&
								   a.IsPresent == true);


				int absentCount = await _context.EmployeeAttendences
					.CountAsync(a => a.EmployeeId == employee.EmployeeId &&
								   a.AttendanceDate >= startDate &&
								   a.AttendanceDate <= endDate &&
								   a.IsAbsent == true);


				int offdaysCount = await _context.EmployeeAttendences
					.CountAsync(a => a.EmployeeId == employee.EmployeeId &&
								   a.AttendanceDate >= startDate &&
								   a.AttendanceDate <= endDate &&
								   a.IsOffday == true);

				decimal calculatedPayableSalary = 0;

				int totalWorkingDays = presentCount + absentCount;
				if (totalWorkingDays > 0)
				{

					calculatedPayableSalary = employee.EmployeeSalary * ((decimal)presentCount / (decimal)totalWorkingDays);
				}

				if (presentCount > 0 || absentCount > 0 || offdaysCount > 0)
				{
					MonthlyAttendanceReportDto reportEntry = new MonthlyAttendanceReportDto
					{
						EmployeeName = employee.EmployeeName,
						MonthName = monthName,
						PresentCount = presentCount,
						AbsentCount = absentCount,
						OffdaysCount = offdaysCount,
						CalculatedPayableSalary = calculatedPayableSalary
					};

					report.Add(reportEntry);
				}
			}

			return report;
		}

		public async Task<List<string>> GetSupervisorHierarchyAsync(int employeeId)
		{
			var hierarchy = new List<string>();
			var currentEmployeeId = employeeId;


			var allEmployees = await _context.Employees.ToListAsync();
			var employeeDict = allEmployees.ToDictionary(e => e.EmployeeId);


			if (!employeeDict.ContainsKey(currentEmployeeId))
			{
				return hierarchy;
			}

			var currentEmployee = employeeDict[currentEmployeeId];
			hierarchy.Add(currentEmployee.EmployeeName);


			while (currentEmployee.SupervisorId.HasValue)
			{
				var supervisorId = currentEmployee.SupervisorId.Value;

				if (!employeeDict.ContainsKey(supervisorId))
				{
					break;
				}

				currentEmployee = employeeDict[supervisorId];
				hierarchy.Add(currentEmployee.EmployeeName);

				if (hierarchy.Count > allEmployees.Count)
				{
					break;
				}
			}

			return hierarchy;
		}
	}
}

