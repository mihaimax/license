using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Subject code cannot exceed 50 characters.")]
        [ForeignKey(nameof(Subject))]
        public string? SubjectCode { get; set; }
        public Subject? Subject { get; set; }

        [Required]
        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        [Required]
        public DateTime AttendanceDateTime { get; set; }

        [Required]
        public int AttendanceStatus { get; set; } 

        [Required]
        public string? Comment { get; set; } 

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
    }
}
