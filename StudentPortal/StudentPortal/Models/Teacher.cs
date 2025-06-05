using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class Teacher : User
    {
        [ForeignKey("User")]
        public int Id { get; set; }
        public string? Specialization { get; set; }
        public string? Position { get; set; }

        [Key]
        public int TeacherId { get; set; }

        public DateTime RegisteredOn { get; set; }
    }
}

