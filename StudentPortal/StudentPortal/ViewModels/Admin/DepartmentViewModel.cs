using StudentPortal.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortal.ViewModels.Admin
{

    public class DepartmentViewModel
    {
       public string? DepartmentCode { get; set; }
       public string? DepartmentName { get; set; }
       public string? DepartmentHead { get; set; }
       public int? DepartmentHeadId { get; set; }
       public string? Phone { get; set; }
       public int TeacherId { get; set; }
       public IFormFile? ExcelFile { get; set; }
        public List<RowResult> ProcessedRows { get; set; } = new();
        public class RowResult
        {
            public int RowNumber { get; set; }
            public bool IsSuccess { get; set; }
            public string Message { get; set; } = string.Empty;
        }

    }
    public class DepartmentsViewModel
    {
        public List<DepartmentViewModel> Department { get; set; }
        public List<DepartmentViewModel.RowResult>? ImportResults { get; set; }

    }
}
