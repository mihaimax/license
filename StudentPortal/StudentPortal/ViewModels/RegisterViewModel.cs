using System.ComponentModel.DataAnnotations;

namespace StudentPortal.ViewModels
{
    public class RegisterViewModel
    {
        //[Display(Name = "Cod Inregistrare")]
        //[Required(ErrorMessage = "Cod required.")]
        public string? RegistrationToken { get; set; }
        [Required(ErrorMessage = "Password required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Password required.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string RepeatPassword { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name required.")]
        public string Name { get; set; }
        [Display(Name = "Surname")]
        [Required(ErrorMessage = "Surname required.")]
        public string Surname { get; set; }
        [Display(Name = "City")]
        [Required(ErrorMessage = "City required.")]
        public string City { get; set; }
        [Display(Name = "County")]
        [Required(ErrorMessage = "County required.")]
        public string County { get; set; }

    }
}
