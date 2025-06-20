namespace StudentPortal.ViewModels.Teacher
{
    public class SituatieViewModel
    {
        public string? Specialization { get; set; }
        public int? Year { get; set; }
        public int? Semester { get; set; }

        public List<StudentDto> Students { get; set; } = new();

        public class StudentDto
        {
            public int StudentId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
        }
    }
}
