using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using StudentPortal.Interfaces;
using StudentPortal.Repositories;

namespace StudentPortal.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ISituationRepository _situationRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly UserManager<Models.User> _userManager;

        public StudentController(ISituationRepository situationRepository, IStudentRepository studentRepository, UserManager<Models.User> userManager)
        {
            _situationRepository = situationRepository;
            _studentRepository = studentRepository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> SituationAsync()
        {
            var userName = User.Identity?.Name ?? string.Empty;
            var situations = await _situationRepository.GetSituationForStudentAsync(userName);
            return View(situations);
        }
        [HttpGet]
        public async Task<IActionResult> DownloadTimetable()
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            var fileNameTask = _studentRepository.GETPDFFileNameAsync(user?.Id);
            var fileName = fileNameTask.Result;
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "timetables");
            var filePath = Path.Combine(uploadsFolder, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                TempData["ErrorMessage"] = "File not found.";
                return RedirectToAction("Index");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", fileName);
        }
    }
}
