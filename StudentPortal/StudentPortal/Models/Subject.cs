using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class Subject
    {
        [Required]
        public string? SubjectName { get; set; }
        [Key]
        [Required]
        [StringLength(50, ErrorMessage = "Subject code cannot exceed 50 characters.")]
        public string? SubjectCode { get; set; }
        [Required]
        public int Credits { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Department code cannot exceed 50 characters.")]
        [ForeignKey(nameof(Department))]
        public string? DepartmentCode { get; set; }

        public Department? Department { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Minimum attendance percentage must be between 0 and 100.")]
        [Precision(5)]
        public decimal MinimumAttendancePercentage { get; set; } = 75.0m; 
        [Required]
        [Range(0, 100, ErrorMessage = "Minimum exam percentage must be between 0 and 100.")]
        [Precision(5)]
        public decimal MinimumExamPercentage { get; set; } = 50.0m;
        [Required]
        [Range(0, 100, ErrorMessage = "Minimum project percentage must be between 0 and 100.")]
        [Precision(5)]
        public decimal MinimumProjectPercentage { get; set; } = 50.0m;

        // New properties for CourseTeacher and LabTeacher
        [ForeignKey(nameof(CourseTeacher))]
        public int? CourseTeacherId { get; set; }
        public Teacher? CourseTeacher { get; set; }

        [ForeignKey(nameof(LabTeacher))]
        public int? LabTeacherId { get; set; }
        public Teacher? LabTeacher { get; set; }
    }
}
