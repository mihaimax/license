
using StudentPortal.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Interfaces;
using StudentPortal.Models;
using static StudentPortal.Models.User;
using StudentPortal.Repositories;
using System.Drawing;
namespace StudentPortal.Controllers

{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITeacherRepository _teacherRepository;

        // <summary>
        /// Initializes a new instance of the AccountController class.
        /// Sets up all required services and repositories for user, student, and teacher management, as well as authentication and database access.
        // </summary>
        public AccountController(ApplicationDbContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUserRepository userRepository,
            IStudentRepository studentRepository, 
            ITeacherRepository teacherRepository)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
        }
        public IActionResult Index()
        {
            return RedirectToAction("LogIn");
        }

        // <summary>
        /// Login action to get the login view.
        /// If the user is already authenticated, it redirects them to their respective dashboard based on their role. (Teacher, Admin, or Student)
        /// If the user is not authenticated, it returns a view for login.
        // </summary>
        [HttpGet]
        public async Task<IActionResult> LogIn()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(" fdasasd fasdfa");
                var res2 = await _userManager.GetRolesAsync(user);
                string role = string.Join(", ", res2);
                if (role == "teacher")
                    return RedirectToAction("Index", "Teacher");
                else if (role == "admin")
                    return RedirectToAction("Index", "Admin");
                else if (role == "student")
                    return RedirectToAction("Index", "Student");
            }
            var response = new LogInViewModel();
            return View();
        }

        // <summary>
        /// Login action method to authenticate users.
        /// Checks if the model state is valid, retrieves the user by email, and checks the account status.
        /// If the user exists and is active, it checks the password and signs in the user.
        /// If the login is successful, it redirects the user to their respective dashboard based on their role.
        /// If the user is not found or the account is inactive, it adds an error to the model state and returns the login view with the error message.
        // </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LogInViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                    }
                }
                return View(loginViewModel);
            }

            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

            //Check if user exists and if the account is active
            if (user != null && (user._accountStatus == AccountStatus.Active))
            {
                //Handle password check
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    //Valid user, handle redirect based on role
                    if (result.Succeeded)
                    {
                        var res2 = await _userManager.GetRolesAsync(user);
                        string role = string.Join(", ", res2);
                        if (role == "Teacher")
                            return RedirectToAction("Index", "Teacher");
                        else if (role == "Admin")
                            return RedirectToAction("Index", "Admin");
                        else if (role == "Student")
                            return RedirectToAction("Index", "Student");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Incorrect password.");
                }
            }
            else if (user != null && user._accountStatus == AccountStatus.PendingActivation)
            {
                ModelState.AddModelError(string.Empty, "Account is not active yet. Follow the registration link sent by e-mail.");
                return View(loginViewModel);
            }
            else if (user != null && user._accountStatus == AccountStatus.Blocked)
                ModelState.AddModelError(string.Empty, "Account is currently blocked.");
            else
            {
                ModelState.AddModelError(string.Empty, "Account not found.");
            }
            return View(loginViewModel);
        }

        // <summary>
        /// Displays the registration view for new users.
        /// If the user is already authenticated, redirects them to their respective dashboard based on their role (Profesor, Admin, Student, or Doctor).
        /// If not authenticated, returns the registration view with a new RegisterViewModel.
        // </summary>
        [HttpGet]
        public async Task<IActionResult> Register(string token)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var res2 = await _userManager.GetRolesAsync(user);

                string role = string.Join(", ", res2);
                if (role == "teacher")
                    return RedirectToAction("Index", "Teacher");
                else if (role == "admin")
                    return RedirectToAction("Index", "Admin");
                else if (role == "student")
                    return RedirectToAction("Index", "Student");
            }

            string id = _userRepository.GetIdByRegistrationToken(token);
            var response = new RegisterViewModel(); 
            if (!string.IsNullOrEmpty(token) && (id != null))
            {
                response.RegistrationToken = token;
                return View(response);
            } else
            {
                return Unauthorized();
            }
        }

        // <summary>
        /// Handles user registration.
        /// Validates the registration token, updates user details, sets the password, and marks the account as active.
        /// If successful, creates a new Teacher record and displays a success message.
        /// If the token is invalid, adds an error to the model state and returns the registration view.
        // </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            string id = _userRepository.GetIdByRegistrationToken(Request.Query["token"]);
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                user.Name = registerViewModel.Name;
                user.Surname = registerViewModel.Surname;
                user.City = registerViewModel.City;
                user.County = registerViewModel.County;
                string userName = user.Name + " " + user.Surname;
                user.UserName = userName;
                user.RegistrationToken = "";
                user._accountStatus = AccountStatus.Active;
                var token2 = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token2, registerViewModel.Password);
                _userRepository.SaveChanges();
                var newTeacher = new Teacher()
                {
                    RegisteredOn = DateTime.Now,
                    Position = "Teacher",
                };
                var newTeacherResponse = _teacherRepository.Add(newTeacher);
                if (newTeacherResponse)
                {
                    _teacherRepository.SaveChanges();
                }
                TempData["Success"] = "Registration complete. You may log in now.";
                return View(registerViewModel);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Registration failed. Make sure all required fields are filled.");
                return View(registerViewModel);
            }
        }
    }
}

