using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Models;
using StudentPortal.ViewModels.Admin;

namespace StudentPortal.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;
        public DepartmentRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            return await _context.Departments.FindAsync(id);
        }

        public async Task<List<DepartmentViewModel>> GetAllViewModelsAsync()
        {
            return await _context.Departments
                .Select(d => new DepartmentViewModel
                {
                    DepartmentCode = d.DepartmentCode,
                    DepartmentName = d.DepartmentName,
                    DepartmentHeadId = d.DepartmentHeadId,
                    Phone = d.Phone
                }).ToListAsync();
        }

        public async Task<Department?> GetByCodeAsync(string departmentCode)
            => await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentCode == departmentCode);

        public async Task<bool> ExistsAsync(string departmentCode)
            => await _context.Departments.AnyAsync(d => d.DepartmentCode == departmentCode);
        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }

        public async Task AddAsync(Department department)
            => await _context.Departments.AddAsync(department);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
