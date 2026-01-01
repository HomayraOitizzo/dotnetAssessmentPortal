using dotnetAssessmentPortal.Exceptions;
using dotnetAssessmentPortal.Models.DTOs;
using dotnetAssessmentPortal.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace dotnetAssessmentPortal.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class EmployeeController : ControllerBase
	{
		private readonly IEmployeeRepository _employeeRepository;

		public EmployeeController(IEmployeeRepository employeeRepository)
		{
			_employeeRepository = employeeRepository;
		}


		[HttpPut("{employeeId:int}")]
		public async Task<IActionResult> UpdateProfile([FromRoute] int employeeId, [FromBody] UpdateEmployeeProfileDto dto)
		{

			if (!ModelState.IsValid)
			{
				throw new BusinessException("Invalid request body", 400);
			}


			var employee = await _employeeRepository.GetByIdAsync(employeeId);
			if (employee == null)
			{
				throw new NotFoundException($"Employee with ID {employeeId} not found");
			}


			bool isCodeTaken = await _employeeRepository.IsCodeAssignedToOtherEmployeeAsync(dto.NewCode, employeeId);
			if (isCodeTaken)
			{
				throw new BusinessException($"Employee code '{dto.NewCode}' is already assigned to another employee", 409);
			}

			employee.EmployeeName = dto.NewName;
			employee.EmployeeCode = dto.NewCode;

			bool updateSuccess = await _employeeRepository.UpdateEmployeeAsync(employee);
			if (!updateSuccess)
			{
				throw new Exception("Failed to update employee profile");
			}

			return Ok(new { message = "Employee profile updated successfully", employeeId = employee.EmployeeId });
		}

		[HttpGet("nth-highest-salary")]
		public async Task<IActionResult> GetNthHighestSalary([FromQuery] int rank)
		{

			if (rank <= 0)
			{
				throw new BusinessException("Rank must be a positive integer", 400);
			}

			var employee = await _employeeRepository.GetNthHighestSalaryEmployeeAsync(rank);

			if (employee == null)
			{
				throw new NotFoundException($"No employee found at rank {rank}");
			}

			return Ok(employee);
		}

		[HttpGet("high-earners-perfect-attendance")]
		public async Task<IActionResult> GetHighEarnersWithPerfectAttendance([FromQuery] decimal minSalary)
		{

			if (minSalary < 0)
			{
				throw new BusinessException("minSalary must be a non-negative value", 400);
			}

			var employees = await _employeeRepository.GetHighEarnersWithPerfectAttendanceAsync(minSalary);

			return Ok(employees);
		}

		[HttpGet("monthly-attendance-report")]
		public async Task<IActionResult> GetMonthlyAttendanceReport([FromQuery] int month, [FromQuery] int year)
		{

			if (month < 1 || month > 12)
			{
				throw new BusinessException("Month must be between 1 and 12", 400);
			}


			if (year < 2000 || year > 2100)
			{
				throw new BusinessException("Year must be between 2000 and 2100", 400);
			}

			var report = await _employeeRepository.GetMonthlyAttendanceReportAsync(month, year);

			return Ok(report);
		}

		[HttpGet("supervisor-hierarchy/{employeeId:int}")]
		public async Task<IActionResult> GetSupervisorHierarchy([FromRoute] int employeeId)

		{

			var employee = await _employeeRepository.GetByIdAsync(employeeId);
			if (employee == null)
			{
				throw new NotFoundException($"Employee with ID {employeeId} not found");
			}

			var hierarchy = await _employeeRepository.GetSupervisorHierarchyAsync(employeeId);

			return Ok(hierarchy);
		}
	}
}

