using StudentPortal.Models;

namespace StudentPortal.Interfaces
{
    public interface IStudentRepository
    {

        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(int id);

        bool Add(Student student);
        bool Update(Student student);
        bool Delete(Student student);
        bool SaveChanges();
    }
}
