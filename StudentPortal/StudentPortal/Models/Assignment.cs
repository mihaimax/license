using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }

        [Required]
        [ForeignKey(nameof(Subject))]
        public string SubjectCode { get; set; } = null!;
        public Subject? Subject { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }
        public DateTime DueDate { get; set; }

        [ForeignKey(nameof(Student))]
        public int? StudentId { get; set; }
        public Student? Student { get; set; }
    }
}
