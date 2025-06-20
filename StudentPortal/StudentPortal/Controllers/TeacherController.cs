using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentPortal.Interfaces;
using StudentPortal.Models;
using StudentPortal.ViewModels.Teacher;

namespace StudentPortal.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISituationRepository _situationRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly UserManager<User> _userManager;

        public TeacherController(ISituationRepository situationRepository, IStudentRepository studentRepository, ApplicationDbContext context, UserManager<User> userManager)
        {
            _situationRepository = situationRepository;
            _studentRepository = studentRepository;
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ManageSituationAsync(string? specialization, int year, int semester)
        {
            var students = await _studentRepository.GetAllStudentsForSituationAsync(specialization, year, semester);

            var viewModel = new SituatieViewModel
            {
                Specialization = specialization,
                Year = year,
                Semester = semester,
                Students = students.Select(s => new SituatieViewModel.StudentDto
                {
                    StudentId = s.StudentId,
                    Name = s.User != null ? s.User.Name ?? "" : "",
                    Surname = s.User != null ? s.User.Surname ?? "" : ""
                }).ToList()
            };

            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> ModifySituation(string? subjectCode, int? studentId)
        {
            var model = new ManageSituationViewModel();
            if (subjectCode.IsNullOrEmpty() && studentId.HasValue)
            {
                model.StudentId = studentId.Value;
                return View(model);
            }

            if (!string.IsNullOrEmpty(subjectCode) && studentId.HasValue)
            {
                model.SubjectCode = subjectCode;
                model.StudentId = studentId.Value;

                // Optionally, load existing situation or other data here
                var situation = await _context.Situations
                    .FirstOrDefaultAsync(s => s.SubjectCode == subjectCode && s.StudentId == studentId.Value);

                if (situation != null)
                {
                    model.TeacherId = situation.TeacherId;
                    model.AttendancePercentage = situation.AttendancePercentage;
                    model.ExamPercentage = situation.ExamPercentage;
                    model.ProjectPercentage = situation.ProjectPercentage;
                    model.FinalGrade = situation.FinalGrade;
                    model.Semester = situation.Semester;
                    model.Year = situation.Year;
                }
                else
                {
                    // Get current teacher
                    var userId = _userManager.GetUserId(User);
                    var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);

                    // Get subject
                    var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.SubjectCode == subjectCode);

                    // Get student
                    var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId.Value);

                    if (teacher != null && subject != null && student != null)
                    {
                        // Create new Situation
                        var newSituation = new Situation
                        {
                            SubjectCode = subjectCode,
                            StudentId = studentId.Value,
                            TeacherId = teacher.TeacherId,
                            AttendancePercentage = 0,
                            ExamPercentage = 0,
                            ProjectPercentage = 0,
                            FinalGrade = 0,
                            Semester = student.Semester ?? 0,
                            Year = student.Year ?? 0
                        };
                        _context.Situations.Add(newSituation);

                        // Create new Enrollment if not exists
                        var enrollment = await _context.Enrollments
                            .FirstOrDefaultAsync(e => e.StudentId == studentId.Value && e.Subject != null && e.Subject.SubjectCode == subjectCode);

                        if (enrollment == null)
                        {
                            var newEnrollment = new Enrollment
                            {
                                StudentId = studentId.Value,
                                SubjectId = subjectCode,
                                Year = student.Year ?? 0,
                                Semester = student.Semester ?? 0
                            };
                            _context.Enrollments.Add(newEnrollment);
                        }

                        await _context.SaveChangesAsync();

                        // Populate model with new situation data
                        model.TeacherId = newSituation.TeacherId;
                        model.AttendancePercentage = newSituation.AttendancePercentage;
                        model.ExamPercentage = newSituation.ExamPercentage;
                        model.ProjectPercentage = newSituation.ProjectPercentage;
                        model.FinalGrade = newSituation.FinalGrade;
                        model.Semester = newSituation.Semester;
                        model.Year = newSituation.Year;
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ModifySituation(ManageSituationViewModel model)
        {
            // Get current teacher's TeacherId
            var userId = _userManager.GetUserId(User);
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
            if (teacher == null)
            {
                ModelState.AddModelError("", "Current user is not a teacher.");
                return View(model);
            }

            // Get subject and check teacher assignment
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.SubjectCode == model.SubjectCode);
            if (subject == null)
            {
                ModelState.AddModelError("", "Subject not found.");
                return View(model);
            }

            if (subject.CourseTeacherId != teacher.TeacherId && subject.LabTeacherId != teacher.TeacherId)
            {
                ModelState.AddModelError("", "You are not assigned as a course or lab teacher for this subject.");
                return View(model);
            }

            // Check enrollment
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == model.StudentId && e.Subject != null && e.Subject.SubjectCode == model.SubjectCode);
            if(enrollment == null)
            {
                var newEnrollment = new Enrollment
                {
                    SubjectId = model.SubjectCode,
                    StudentId = model.StudentId,
                    Year = 0,
                    Semester = 0
                };
                _context.Enrollments.Add(newEnrollment);
                _context.SaveChanges();
            }
            enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == model.StudentId && e.Subject != null && e.Subject.SubjectCode == model.SubjectCode);
            if (enrollment != null)
            {
                // Optionally, load more data from Situation or other tables if needed
                var situation = await _context.Situations
                    .FirstOrDefaultAsync(s => s.StudentId == model.StudentId && s.SubjectCode == model.SubjectCode);
                situation.AttendancePercentage = model.AttendancePercentage;
                situation.ExamPercentage = model.ExamPercentage;
                situation.ProjectPercentage = model.ProjectPercentage;
                situation.FinalGrade = model.FinalGrade;
                situation.Semester = model.Semester;
                situation.Year = model.Year;
                situation.TeacherId = teacher.TeacherId;
                await _context.SaveChangesAsync();
                ViewBag.SuccessMessage = "Situation updated successfully.";
                return View(model);
            }
            else
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == model.StudentId);

                // Create a blank Situation if not enrolled
                var situation = new Situation
                {
                    SubjectCode = model.SubjectCode,
                    StudentId = model.StudentId,
                    TeacherId = teacher.TeacherId,
                    AttendancePercentage = 0,
                    ExamPercentage = 0,
                    ProjectPercentage = 0,
                    FinalGrade = 0,
                    Semester = student?.Semester ?? 0, // Use student's Semester, or 0 if null
                    Year = student?.Year ?? 0
                };

                _context.Situations.Add(situation);
                await _context.SaveChangesAsync();

                model.TeacherId = situation.TeacherId;
                model.AttendancePercentage = situation.AttendancePercentage;
                model.ExamPercentage = situation.ExamPercentage;
                model.ProjectPercentage = situation.ProjectPercentage;
                model.FinalGrade = situation.FinalGrade;
                model.Semester = situation.Semester;
                model.Year = situation.Year;

                return View(model);
            }
        }
    }
}
