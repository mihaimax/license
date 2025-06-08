using System.ComponentModel.DataAnnotations;

namespace StudentPortal.ViewModels.Account
{
    public class LogInViewModel
    {
            [Display(Name = "E-mail Address")]
            [Required(ErrorMessage = "E-mail required.")]
            public string Email { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
    }
}
