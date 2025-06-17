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
        public string? Email { get; set; }

        public UserFunction? Function { get; set; }
        public AccountStatus? AccountStatus { get; set; }
        [Required(ErrorMessage = "Please select an Excel file")]
        public IFormFile? ExcelFile { get; set; }
        public List<RowResult> ProcessedRows { get; set; } = new();
        public class RowResult
        {
            public int RowNumber { get; set; }
            public bool IsSuccess { get; set; }
            public string Message { get; set; } = string.Empty;
        }
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
        public List<UserViewModel.RowResult>? ImportResults { get; set; }
    }
}