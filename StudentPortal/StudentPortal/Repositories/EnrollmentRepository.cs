using Microsoft.EntityFrameworkCore;
using StudentPortal.Interfaces;
using StudentPortal.Models;
using StudentPortal.ViewModels.Admin;

namespace StudentPortal.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ApplicationDbContext _context;
        public EnrollmentRepository(ApplicationDbContext context) => _context = context;
        public async Task<List<EnrollmentViewModel>> GetAllViewModelsAsync()
        {
            return await _context.Enrollments
                .Select(d => new EnrollmentViewModel
                {
                    EnrollmentId = d.EnrollmentId,
                    StudentId = d.StudentId,
                    SubjectId = d.SubjectId,
                    Year = d.Year,
                    Semester = d.Semester
                }).ToListAsync();
        }

        public Task<bool> EnrollmentExists(int studentId, string subjectId, int year, int semester)
        {
            return _context.Enrollments.AnyAsync(e => e.StudentId == studentId && e.SubjectId == subjectId && e.Year == year && e.Semester == semester);
        }
        public async Task<bool> AddAsync(Enrollment enrollment)
        {
            await _context.Enrollments.AddAsync(enrollment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
