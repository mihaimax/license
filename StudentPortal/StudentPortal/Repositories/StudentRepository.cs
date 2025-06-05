using StudentPortal.Interfaces;
using StudentPortal.Models;
using System.Data.Entity;
namespace StudentPortal.Repositories

{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;
        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _context.Students.ToListAsync();
        }
        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.Students.FindAsync(id);
        }
        public bool Add(Student student)
        {
            _context.Students.Add(student);
            return SaveChanges();
        }
        public bool Update(Student student)
        {
            _context.Students.Update(student);
            return SaveChanges();
        }
        public bool Delete(Student student)
        {
            _context.Students.Remove(student);
            return SaveChanges();
        }
        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }
    }


}
