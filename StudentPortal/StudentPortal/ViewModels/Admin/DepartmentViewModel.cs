using StudentPortal.Models;
using System.Collections.Generic;

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
    }
    public class DepartmentsViewModel
    {
        public List<DepartmentViewModel> Department { get; set; }
    }
}
