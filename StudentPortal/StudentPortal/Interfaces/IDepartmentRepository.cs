using StudentPortal.Models;
using StudentPortal.ViewModels.Admin;

public interface IDepartmentRepository
{
    Task<List<DepartmentViewModel>> GetAllViewModelsAsync();
    Task<Department?> GetByCodeAsync(string departmentCode);
    Task<bool> ExistsAsync(string departmentCode);
    Task AddAsync(Department department);
    Task SaveChangesAsync();
}