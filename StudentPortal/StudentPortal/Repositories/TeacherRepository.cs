using StudentPortal.Interfaces;
using StudentPortal.Models;
using System.Data.Entity;
namespace StudentPortal.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ApplicationDbContext _context;
        public TeacherRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
        {
            return await _context.Teachers.ToListAsync();
        }
        public async Task<Teacher?> GetTeacherByIdAsync(int id)
        {
            return await _context.Teachers.FindAsync(id);
        }
        public bool Add(Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            return SaveChanges();
        }
        public bool Update(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
            return SaveChanges();
        }
        public bool Delete(Teacher teacher)
        {
            _context.Teachers.Remove(teacher);
            return SaveChanges();
        }
        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }
    }


}
