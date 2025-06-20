namespace StudentPortal.ViewModels.Student
{
    public class SituationViewModel
    {
        public int Year { get; set; }
        public int Semester { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public decimal AttendancePercentage { get; set; }
        public decimal ExamPercentage { get; set; }
        public decimal ProjectPercentage { get; set; }
        public decimal FinalGrade { get; set; }
        public string? Specialization { get; set; }

        public List<StudentDto> Students { get; set; } = new();

        public class StudentDto
        {
            public int StudentId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
        }
    }
}