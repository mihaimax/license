namespace StudentPortal.ViewModels.Teacher
{
    public class ManageSituationViewModel
    {
        public string SubjectCode { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
        public decimal AttendancePercentage { get; set; }
        public decimal ExamPercentage { get; set; }
        public decimal ProjectPercentage { get; set; }
        public decimal FinalGrade { get; set; }
        public int Semester { get; set; }
        public int Year { get; set; }
    }
}
