using Microsoft.AspNetCore.Identity;
using StudentPortal.Interfaces;
using StudentPortal.Models;
using StudentPortal.ViewModels.Student;
using Microsoft.EntityFrameworkCore;

namespace StudentPortal.Repositories
{
    public class SituationRepository: ISituationRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly IStudentRepository _studentRepository;
        private readonly ApplicationDbContext _context;
        public SituationRepository(ApplicationDbContext context, UserManager<User> userManager, IStudentRepository studentRepository)
        {
            _context = context;
            _userManager = userManager;
            _studentRepository = studentRepository;
        }

        public async Task<List<SituationViewModel>> GetSituationForStudentAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var studentId = await _studentRepository.GetStudentIdByUserNameAsync(user.Id);
            var situations = await _context.Situations
                .Where(s => s.StudentId == studentId)
                .OrderBy(s => s.Year)
                .ThenBy(s => s.Semester)
                .ToListAsync();

            var viewModels = situations.Select(s => new SituationViewModel
            {
                Year = s.Year,
                Semester = s.Semester,
                SubjectCode = s.SubjectCode ?? "",
                AttendancePercentage = s.AttendancePercentage,
                ExamPercentage = s.ExamPercentage,
                ProjectPercentage = s.ProjectPercentage,
                FinalGrade = s.FinalGrade
            }).ToList();

            return viewModels;
        }
    }
}
