using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class Student : User
    {
        [ForeignKey("User")]
        public int Id { get; set; }

        [Key]
        public string StudentNumber { get; set; }

        public string Specialization { get; set; }

        public int Year { get; set; }

        public string Semester { get; set; }

        public string Group { get; set; }

        public DateTime RegisteredOn { get; set; }
    }
}
