using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Interfaces;
using StudentPortal.Models;
using StudentPortal.ViewModels.Admin;
using StudentPortal.Classes;

namespace StudentPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IConfiguration _configuration;

        // <summary>
        /// Initializes a new instance of the AdminController class.
        /// Sets up all required services and repositories for user, student, and teacher management, as well as authentication and database access.
        // </summary>
        public AdminController(ApplicationDbContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUserRepository userRepository,
            IStudentRepository studentRepository,
            ITeacherRepository teacherRepository,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult InviteUser()
        {
            var response = new InviteUserViewModel();
            return View(response);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InviteUser(InviteUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "A user with this email already exists.");
                    return View(model);
                }
                var user = new User
                {
                    Email = model.Email,
                    UserName = model.Email,
                    _accountStatus = Models.User.AccountStatus.PendingActivation,
                    RegistrationToken = Guid.NewGuid().ToString(),
                    _function = (User.Function?)model.UserType
                };

                var result = await _userManager.CreateAsync(user, Guid.NewGuid().ToString());
                if (result.Succeeded)
                {
                    await SendInvitationEmailAsync(model.Email, user.RegistrationToken);
                    TempData["Success"] = "The invitation has been successfully sent.";
                    return View(model);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        private async Task SendInvitationEmailAsync(string toEmail, string registrationToken)
        {
            var registrationUrl = Url.Action("Register", "Account", new { token = registrationToken }, protocol: Request.Scheme);
            var subject = "Portal Registration Invitation";
            var body = $"You have been invited to register. Please click the link to complete your registration: <a href=\"{registrationUrl}\">{registrationUrl}</a>";
            var mailManager = new MailManager(_configuration);
            await mailManager.SendEmailAsync(toEmail, subject, body);
        }
    }
}
