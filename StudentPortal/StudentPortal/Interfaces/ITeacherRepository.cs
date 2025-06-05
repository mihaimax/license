using StudentPortal.Models;

namespace StudentPortal.Interfaces
{
    public interface ITeacherRepository
    {
        Task<IEnumerable<Teacher>> GetAllTeachersAsync();
        Task<Teacher?> GetTeacherByIdAsync(int id);

        bool Add(Teacher teacher);
        bool Update(Teacher teacher);
        bool Delete(Teacher teacher);
        bool SaveChanges();
    }
}
