using Microsoft.EntityFrameworkCore.Storage;
using StudentPortal.Data;
using StudentPortal.Interfaces;
using System.Threading.Tasks;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IUserRepository _users;
    private readonly IStudentRepository _students;
    private readonly ITeacherRepository _teachers;
    private readonly IDepartmentRepository _departments;
    private readonly ISubjectRepository _subjects;
    private readonly ITimeTableRepository _timeTables;

    public IUserRepository Users => _users;
    public IStudentRepository Students => _students;
    public ITeacherRepository Teachers => _teachers;
    public IDepartmentRepository Departments => _departments;
    public ISubjectRepository Subjects => _subjects;
    public ITimeTableRepository TimeTables => _timeTables;

    public UnitOfWork(
        ApplicationDbContext dbContext,
        IUserRepository users,
        IStudentRepository students,
        ITeacherRepository teachers,
        IDepartmentRepository departments,
        ISubjectRepository subjects,
        ITimeTableRepository timeTables)
    {
        _dbContext = dbContext;
        _users = users;
        _students = students;
        _teachers = teachers;
        _departments = departments;
        _subjects = subjects;
        _timeTables = timeTables;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
        => await _dbContext.Database.BeginTransactionAsync();

    public async Task<int> SaveChangesAsync()
        => await _dbContext.SaveChangesAsync();

    public async Task CommitAsync()
    {
        var transaction = _dbContext.Database.CurrentTransaction;
        if (transaction != null)
        {
            await transaction.CommitAsync();
        }
    }

    public async Task RollbackAsync()
    {
        var transaction = _dbContext.Database.CurrentTransaction;
        if (transaction != null)
        {
            await transaction.RollbackAsync();
        }
    }
}