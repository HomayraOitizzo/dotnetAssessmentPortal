using System.ComponentModel.DataAnnotations;

namespace dotnetAssessmentPortal.Models.DTOs
{
    public class UpdateEmployeeProfileDto
    {
        [Required(ErrorMessage = "NewName is required")]
        [StringLength(100, ErrorMessage = "NewName cannot exceed 100 characters")]
        public required string NewName { get; set; }

        [Required(ErrorMessage = "NewCode is required")]
        [StringLength(50, ErrorMessage = "NewCode cannot exceed 50 characters")]
        public required string NewCode { get; set; }
    }
}

