using StudentPortal.Models;

namespace StudentPortal.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(int id);
        Task<bool> AddAsync(Student student);
        Task<bool> ExistsAsync(int studentId);
        Task SaveChangesAsync();
        Task<int> GetStudentIdByUserNameAsync(string userName);
        Task<string> GETPDFFileNameAsync(string userName);
        Task<IEnumerable<Student>> GetAllStudentsForSituationAsync(string specialization, int year, int semester);
    }
}
