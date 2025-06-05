using System.ComponentModel.DataAnnotations;

namespace StudentPortal.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Registration Token")]
        [Required(ErrorMessage = "Token Necessary")]
        public string RegistrationToken { get; set; }

        [Required(ErrorMessage = "Field must filled")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Passwords Do Not Match")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords Do Not Match")]
        public string RepeatPassword { get; set; }

    }
}
