using System.ComponentModel.DataAnnotations;
using static StudentPortal.Models.User;

namespace StudentPortal.ViewModels.Admin
{
    public class BulkInviteViewModel
    {
        [Required(ErrorMessage = "Please select an Excel file")]
        public IFormFile? ExcelFile { get; set; }

        public List<InviteResult> ProcessedInvites { get; set; } = new();

        public class InviteResult
        {
            public string Email { get; set; } = string.Empty;
            public Function UserType { get; set; }
            public bool IsSuccess { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }
}
