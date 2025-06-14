using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace StudentPortal.ViewModels.Admin
{
    public class BulkImportSubjectsViewModel
    {
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
}
