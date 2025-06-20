namespace StudentPortal.ViewModels.Admin
{
    public class EnrollmentViewModel
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public string SubjectId { get; set; }
        public int Year { get; set; }
        public int Semester { get; set; }
        public IFormFile? ExcelFile { get; set; }
        public List<EnrollmentViewModel.RowResult> ProcessedRows { get; set; }
        public class RowResult
        {
            public int RowNumber { get; set; }
            public bool IsSuccess { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }
    public class EnrollmentsViewModel
    {
        public List<EnrollmentViewModel> Enrollment { get; set; }
        public List<EnrollmentViewModel.RowResult>? ImportResults { get; set; }
    }
}
