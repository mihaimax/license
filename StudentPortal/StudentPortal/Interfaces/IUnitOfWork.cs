using Microsoft.EntityFrameworkCore.Storage;
using StudentPortal.Interfaces;
using System.Threading.Tasks;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IStudentRepository Students { get; }
    ITeacherRepository Teachers { get; }
    IDepartmentRepository Departments { get; }
    ISubjectRepository Subjects { get; }
    ITimeTableRepository TimeTables { get; }
    IEnrollmentRepository Enrollments { get; }

    Task<IDbContextTransaction> BeginTransactionAsync();
    Task<int> SaveChangesAsync();
    Task CommitAsync();
    Task RollbackAsync();
}