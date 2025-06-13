using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortal.ViewModels.Admin
{
    public class UserViewModel
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Name { get; set; }

        [Required]
        public string? Surname { get; set; }

        public string? City { get; set; }
        public string? County { get; set; }
        public string? Address { get; set; }
        public string? CNP { get; set; }

        public UserFunction? Function { get; set; }
        public AccountStatus? AccountStatus { get; set; }
    }

    public enum UserFunction
    {
        Student,
        Teacher,
        Admin
    }

    public enum AccountStatus
    {
        Active,
        PendingActivation,
        Blocked
    }

    public class UsersViewModel
    {
        public List<UserViewModel> Users { get; set; } = new();
    }
}