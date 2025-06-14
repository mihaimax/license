using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Models;
using StudentPortal.ViewModels.Admin;

namespace StudentPortal.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly ApplicationDbContext _context;
        public SubjectRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Subject>> GetAllSubjectsAsync()
        {
            return await _context.Subjects.ToListAsync();
        }

        public async Task<Subject?> GetSubjectByIdAsync(int id)
        {
            return await _context.Subjects.FindAsync(id);
        }

        public bool Add(Subject subject)
        {
            _context.Subjects.Add(subject);
            return SaveChanges();
        }

        public bool Update(Subject subject)
        {
            _context.Subjects.Update(subject);
            return SaveChanges();
        }

        public bool Delete(Subject subject)
        {
            _context.Subjects.Remove(subject);
            return SaveChanges();
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }

        public async Task<List<SubjectViewModel>> GetAllViewModelsAsync()
        {
            return await _context.Subjects
                .Select(s => new SubjectViewModel
                {
                    SubjectCode = s.SubjectCode,
                    SubjectName = s.SubjectName,
                    DepartmentCode = s.DepartmentCode,
                    MinAttendancePercentage = s.MinimumAttendancePercentage,
                    MinExamPercentage = s.MinimumExamPercentage,
                    MinLabPercentage = s.MinimumProjectPercentage,
                    Credits = s.Credits,
                    CourseTeacherId = s.CourseTeacherId,
                    LabTeacherId = s.LabTeacherId
                }).ToListAsync();
        }

        public async Task<Subject?> GetByCodeAsync(string subjectCode)
            => await _context.Subjects.FirstOrDefaultAsync(s => s.SubjectCode == subjectCode);

        public async Task<bool> ExistsAsync(string subjectCode)
            => await _context.Subjects.AnyAsync(s => s.SubjectCode == subjectCode);

        public async Task AddAsync(Subject subject)
            => await _context.Subjects.AddAsync(subject);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
