using StudentPortal.Models;
using System.Collections.Generic;

namespace StudentPortal.ViewModels.Admin
{
    public class TimeTableViewModel
    {
        public List<TimeTable> TimeTable { get; set; } = new();
        public string Weekday { get; set; }
        public string? DepartmentCode { get; set; }
        public string? DepartmentName { get; set; }
        public int Year { get; set; }
        public int Semester { get; set; }
        public string? SubjectCode { get; set; }
        public string? SubjectName { get; set; }
        public bool IsLab { get; set; }
        public string? CourseTeacherName { get; set; }
        public string? LabTeacherName { get; set; }
        public int? LabTeacherId { get; set; }           // <-- Add this
        public int? CourseTeacherId { get; set; }        // <-- Add this
        public TimeOnly? StartTime { get; set; }         // <-- Add this
        public TimeOnly? EndTime { get; set; }
        public IFormFile? ExcelFile { get; set; }
        public List<RowResult> ProcessedRows { get; set; } = new();
        public class RowResult
        {
            public int RowNumber { get; set; }
            public bool IsSuccess { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }
    public class TimeTablesViewModel
    {
        public List<TimeTableViewModel> TimeTable { get; set; } = new List<TimeTableViewModel>();
        public List<TimeTableViewModel.RowResult>? ImportResults { get; set; }
    }
}
