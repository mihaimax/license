using StudentPortal.Models;
using StudentPortal.Repositories;
using StudentPortal.ViewModels.Admin;

namespace StudentPortal.Interfaces
{
    public interface IEnrollmentRepository
    {
        public Task<List<EnrollmentViewModel>> GetAllViewModelsAsync();
        public Task<bool> EnrollmentExists(int studentId, string subjectId, int year, int semester);
        public Task<bool> AddAsync(Enrollment enrollment);

        public Task SaveChangesAsync();
    }
}
