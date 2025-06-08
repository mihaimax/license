using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    [PrimaryKey(nameof(SubjectCode), nameof(StudentId))]
    public class Situation
    {
        [Key, Column(Order = 0)]
        [Required]
        [StringLength(50, ErrorMessage = "Subject code cannot exceed 50 characters.")]
        [ForeignKey(nameof(Subject))]
        public string? SubjectCode { get; set; }
        public Subject? Subject { get; set; }

        [Key, Column(Order = 1)]
        [Required]
        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        [Required]
        [ForeignKey(nameof(Teacher))]
        public int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Attendance percentage must be between 0 and 100.")]
        [Precision(5)]
        public decimal AttendancePercentage { get; set; } = 0.0m;
        [Required]
        [Range(0, 100, ErrorMessage = "Exam percentage must be between 0 and 100.")]
        [Precision(5)]
        public decimal ExamPercentage { get; set; } = 0.0m;
        [Required]
        [Range(0, 100, ErrorMessage = "Project percentage must be between 0 and 100.")]
        [Precision(5)]
        public decimal ProjectPercentage { get; set; } = 0.0m;
        [Required]
        [Range(0, 100, ErrorMessage = "Final Grade must be between 0 and 100.")]
        [Precision(5)]
        public decimal FinalGrade { get; set; } = 0.0m;
    }
}
