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

        public enum Function
        {
            Student,
            Teacher,
            Admin
        }

        public Function? _function { get; set; }
        public enum AccountStatus
        {
            Active,
            PendingActivation,
            Blocked
        }
        public AccountStatus? _accountStatus { get; set; }

        public string? RegistrationToken { get; set; }

        public string? County { get; set; }

        public string? City { get; set; }

        public string? Address { get; set; }

        public string? CNP { get; set; }
    }
}
