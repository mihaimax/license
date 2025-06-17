using StudentPortal.Interfaces;
using StudentPortal.Models;
using StudentPortal.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;
namespace StudentPortal.Repositories
{
    public class TimeTableRepository: ITimeTableRepository
    {
        private readonly ApplicationDbContext _context;
        public TimeTableRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<TimeTable>> GetAllTimeTablesAsync()
        {
            return await _context.TimeTables.ToListAsync();
        }
        public async Task<TimeTable?> GetTimeTableByPKAsync(
            string departmentCode,
            int year,
            int semester,
            string subjectCode,
            string weekday,
            TimeOnly startTime,
            TimeOnly endTime)
        {
            return await _context.TimeTables.FindAsync(
                departmentCode, year, semester, subjectCode, weekday, startTime, endTime);
        }
        public async Task AddAsync(TimeTable timeTable)
        {
            await _context.TimeTables.AddAsync(timeTable); // Use AddAsync instead of Add
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<TimeTableViewModel>> GetAllViewModelsAsync()
        {
            return await _context.TimeTables
                .Select(t => new TimeTableViewModel
                {
                    DepartmentCode = t.Department != null ? t.Department.DepartmentCode : null,
                    DepartmentName = t.Department != null ? t.Department.DepartmentName : null,
                    Weekday = t.Weekday,
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    Year = t.Year,
                    Semester = t.Semester,
                    SubjectCode = t.Subject != null ? t.Subject.SubjectCode : null,
                    SubjectName = t.Subject != null ? t.Subject.SubjectName : null,
                    IsLab = t.IsLab,
                    LabTeacherName = t.LabTeacher != null ? t.LabTeacher.User.Name + " " + t.LabTeacher.User.Surname : null,
                })
                .ToListAsync();
        }
    }
}
