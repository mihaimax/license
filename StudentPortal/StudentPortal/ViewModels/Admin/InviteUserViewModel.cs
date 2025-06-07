using System.ComponentModel.DataAnnotations;

namespace StudentPortal.ViewModels.Admin
{
    public class InviteUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public Function UserType { get; set; }

        public enum Function
        {
            Student,
            Teacher,
            Admin
        }
    }
}