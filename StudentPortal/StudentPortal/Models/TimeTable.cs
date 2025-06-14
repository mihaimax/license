using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    [PrimaryKey(nameof(DepartmentCode), nameof(Year), nameof(Semester), nameof(SubjectCode))]
    public class TimeTable
    {
        [Required]
        [StringLength(50)]
        [ForeignKey(nameof(Department))]
        public string DepartmentCode { get; set; } = null!;
        public Department? Department { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int Semester { get; set; }

        [Required]
        [StringLength(50)]
        [ForeignKey(nameof(Subject))]
        public string SubjectCode { get; set; } = null!;
        public Subject? Subject { get; set; }
        [Required]
        [StringLength(100)]
        public string Weekday { get; set; } = null!;
        [Required]
        public TimeOnly StartTime { get; set; }
        [Required]
        public TimeOnly EndTime { get; set; }

        [Required]
        public bool IsLab { get; set; }

        [ForeignKey(nameof(LabTeacher))]
        public int? LabTeacherId { get; set; }
        public Teacher? LabTeacher { get; set; }

        [ForeignKey(nameof(CourseTeacher))]
        public int? CourseTeacherId { get; set; }
        public Teacher? CourseTeacher { get; set; }
        public string Specialization { get; set; } = null!;
    }
}
