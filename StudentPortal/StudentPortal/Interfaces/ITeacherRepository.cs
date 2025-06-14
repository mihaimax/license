using StudentPortal.Models;

namespace StudentPortal.Interfaces
{
    public interface ITeacherRepository
    {
        Task<IEnumerable<Teacher>> GetAllTeachersAsync();
        Task<Teacher?> GetTeacherByIdAsync(int id);
        Task<bool> AddAsync(Teacher teacher);
        Task<bool> SaveChangesAsync();
    }
}
