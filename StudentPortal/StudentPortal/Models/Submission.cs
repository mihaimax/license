using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class Submission
    {
        [Key]
        public int SubmissionId { get; set; }

        [Required]
        [ForeignKey(nameof(Assignment))]
        public int AssignmentId { get; set; }
        public Assignment? Assignment { get; set; }

        [Required]
        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public DateTime SubmittedOn { get; set; }
        public string? FilePath { get; set; }
        public string? Comments { get; set; }
        public DateTime GradedOn { get; set; } = DateTime.Now;
        [Precision(5)]
        [Range(0, 100, ErrorMessage = "Grade must be between 0 and 100.")]
        public decimal Grade { get; set; }
    }
}
