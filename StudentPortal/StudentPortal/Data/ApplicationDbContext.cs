using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<Situation> Situations { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<TimeTable> TimeTables { get; set; }
    public DbSet<Submission> Submissions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Student>()
            .HasOne(s => s.User)
            .WithOne(u => u.Student)
            .HasForeignKey<Student>(s => s.UserId)
            .IsRequired();
        modelBuilder.Entity<Teacher>()
            .HasOne(s => s.User)
            .WithOne(u => u.Teacher)
            .HasForeignKey<Teacher>(s => s.UserId)
            .IsRequired();

        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Student)
            .WithMany()
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Subject)
            .WithMany()
            .HasForeignKey(a => a.SubjectCode)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Situation>()
            .HasOne(s => s.Subject)
            .WithMany()
            .HasForeignKey(s => s.SubjectCode)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Situation>()
            .HasOne(s => s.Student)
            .WithMany()
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Situation>()
            .HasOne(s => s.Teacher)
            .WithMany()
            .HasForeignKey(s => s.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
        // TimeTable: Prevent multiple cascade paths
        modelBuilder.Entity<TimeTable>()
            .HasOne(tt => tt.Department)
            .WithMany()
            .HasForeignKey(tt => tt.DepartmentCode)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TimeTable>()
            .HasOne(tt => tt.Subject)
            .WithMany()
            .HasForeignKey(tt => tt.SubjectCode)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TimeTable>()
            .HasOne(tt => tt.LabTeacher)
            .WithMany()
            .HasForeignKey(tt => tt.LabTeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TimeTable>()
            .HasOne(tt => tt.CourseTeacher)
            .WithMany()
            .HasForeignKey(tt => tt.CourseTeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TimeTable>()
            .HasKey(t => new { t.DepartmentCode, t.Year, t.Semester, t.SubjectCode, t.Weekday, t.StartTime, t.EndTime });

        // Assignment: Student is optional, restrict delete
        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Student)
            .WithMany()
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Subject)
            .WithMany()
            .HasForeignKey(a => a.SubjectCode)
            .OnDelete(DeleteBehavior.Restrict);

        // Submission: Student restrict, Assignment cascade
        modelBuilder.Entity<Submission>()
            .HasOne(s => s.Student)
            .WithMany()
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Submission>()
            .HasOne(s => s.Assignment)
            .WithMany()
            .HasForeignKey(s => s.AssignmentId)
            .OnDelete(DeleteBehavior.Restrict);
        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys())
            .Where(fk => fk.PrincipalEntityType.ClrType == typeof(Student)))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys())
            .Where(fk => fk.PrincipalEntityType.ClrType == typeof(Teacher)))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}