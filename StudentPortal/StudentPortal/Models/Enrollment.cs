using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class Enrollment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EnrollmentId { get; set; }

        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }

        [ForeignKey(nameof(Subject))]
        public string SubjectId { get; set; }

        public int Year { get; set; }
        public int Semester { get; set; }

        public Student? Student { get; set; }

        public Subject? Subject { get; set; }
    }
}
