using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Models
{
    public class Teacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TeacherId { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
        public string? Position { get; set; }
        public DateTime RegisteredOn { get; set; }
    }
}

    