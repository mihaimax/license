using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace StudentPortal.Models
{
    [Table("AspNetUsers")]
    public class User : IdentityUser
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Surname { get; set; }

        public enum Status
        {
            Student,
            Teacher,
            Admin
        }

        public Status _status { get; set; }

        public string? RegistrationToken { get; set; }

        public string? County { get; set; }

        public string? City { get; set; }

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        public string? CNP { get; set; }

        public string Email { get; set; }

        [Key]
        public int Id { get; set; }




    }
}
