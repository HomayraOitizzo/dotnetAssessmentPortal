using dotnetAssessmentPortal.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace dotnetAssessmentPortal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        public DbSet <Employee> Employees { get; set; }
		public DbSet<EmployeeAttendence> EmployeeAttendences { get; set; }
	}
}

