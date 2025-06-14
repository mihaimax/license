using StudentPortal.Models;
using StudentPortal.ViewModels.Admin;

public interface ISubjectRepository
{
    Task<List<SubjectViewModel>> GetAllViewModelsAsync();
    Task<Subject?> GetByCodeAsync(string subjectCode);
    Task<bool> ExistsAsync(string subjectCode);
    Task AddAsync(Subject subject);
    Task SaveChangesAsync();
}