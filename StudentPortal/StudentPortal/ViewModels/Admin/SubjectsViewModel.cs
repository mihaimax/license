using System.Collections.Generic;

namespace StudentPortal.ViewModels.Admin
{
    public class SubjectViewModel
    {
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public decimal MinAttendancePercentage { get; set; }
        public decimal MinExamPercentage { get; set; }
        public decimal MinLabPercentage { get; set; }
        public int Credits { get; set; }
    }

    public class SubjectsViewModel
    {
        public List<SubjectViewModel> Subjects { get; set; } = new();
    }
}
