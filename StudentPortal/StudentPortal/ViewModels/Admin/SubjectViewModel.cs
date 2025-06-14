using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortal.ViewModels.Admin
{
    public class SubjectViewModel
    {
        [Required(ErrorMessage = "Subject code is required.")]
        [StringLength(20, ErrorMessage = "Subject code cannot exceed 20 characters.")]
        public string SubjectCode { get; set; }

        [Required(ErrorMessage = "Subject name is required.")]
        [StringLength(100, ErrorMessage = "Subject name cannot exceed 100 characters.")]
        public string SubjectName { get; set; }

        [Required(ErrorMessage = "Department code is required.")]
        [StringLength(20, ErrorMessage = "Department code cannot exceed 20 characters.")]
        public string DepartmentCode { get; set; }

        [Required(ErrorMessage = "Minimum attendance percentage is required.")]
        [Range(0, 100, ErrorMessage = "Attendance percentage must be between 0 and 100.")]
        public decimal MinAttendancePercentage { get; set; }

        [Required(ErrorMessage = "Minimum exam percentage is required.")]
        [Range(0, 100, ErrorMessage = "Exam percentage must be between 0 and 100.")]
        public decimal MinExamPercentage { get; set; }

        [Required(ErrorMessage = "Minimum lab percentage is required.")]
        [Range(0, 100, ErrorMessage = "Lab percentage must be between 0 and 100.")]
        public decimal MinLabPercentage { get; set; }

        [Required(ErrorMessage = "Credits are required.")]
        [Range(1, 60, ErrorMessage = "Credits must be between 1 and 60.")]
        public int Credits { get; set; }

        // Allow null or positive numbers for teacher IDs
        public int? CourseTeacherId { get; set; }

        // Allow null or positive numbers for teacher IDs
        public int? LabTeacherId { get; set; }
        public IFormFile? ExcelFile { get; set; }

        public List<RowResult> ProcessedRows { get; set; } = new();

        public class RowResult
        {
            public int RowNumber { get; set; }
            public bool IsSuccess { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }

    public class SubjectsViewModel
    {
        public List<SubjectViewModel> Subject { get; set; } = new List<SubjectViewModel>();
        public List<SubjectViewModel.RowResult>? ImportResults { get; set; }
    }
}
