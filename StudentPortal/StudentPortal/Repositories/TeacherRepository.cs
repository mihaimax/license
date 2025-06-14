using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Interfaces;
using StudentPortal.Models;

namespace StudentPortal.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ApplicationDbContext _context;
        public TeacherRepository(ApplicationDbContext context) => _context = context;

        public async Task<Teacher?> GetByIdAsync(int teacherId)
            => await _context.Teachers.FirstOrDefaultAsync(t => t.TeacherId == teacherId);

        public async Task<List<Teacher>> GetAllAsync()
            => await _context.Teachers.ToListAsync();


        public async Task<bool> AddAsync(Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        // Fix for CS0535: Implementing GetAllTeachersAsync
        public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
            => await _context.Teachers.ToListAsync();

        // Fix for CS0535: Implementing GetTeacherByIdAsync
        public async Task<Teacher?> GetTeacherByIdAsync(int id)
            => await _context.Teachers.FirstOrDefaultAsync(t => t.TeacherId == id);
    }
}
