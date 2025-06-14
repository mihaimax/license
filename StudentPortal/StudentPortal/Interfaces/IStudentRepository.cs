using StudentPortal.Models;

namespace StudentPortal.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(int id);
        Task<bool> AddAsync(Student student);
        Task SaveChangesAsync();
    }
}
