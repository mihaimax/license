using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public string Specialization { get; set; }
        public int Year { get; set; }
        public string? Semester { get; set; }
        public string? Group { get; set; }
        public DateTime RegisteredOn { get; set; }
    }
}
