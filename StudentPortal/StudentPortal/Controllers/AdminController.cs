using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OfficeOpenXml;
using StudentPortal.Classes;
using StudentPortal.Interfaces;
using StudentPortal.Models;
using StudentPortal.Services;
using StudentPortal.ViewModels.Admin;
using static StudentPortal.Models.User;

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
        private readonly RegistrationNumberGenerator _regNumberGenerator;

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
            IConfiguration configuration,
            RegistrationNumberGenerator regNumberGenerator)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _configuration = configuration;
            _regNumberGenerator = regNumberGenerator;
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
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var token = Guid.NewGuid().ToString();
                    var user = new User
                    {
                        Email = model.Email,
                        Name = "temp",
                        Surname = "temp",
                        UserName = model.Email,
                        _accountStatus = Models.User.AccountStatus.PendingActivation,
                        RegistrationToken = token,
                        _function = (User.Function?)model.UserType
                    };

                    var userResult = await _userManager.CreateAsync(user, Guid.NewGuid().ToString());
                    if (!userResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        ModelState.AddModelError(string.Empty, "Error while creating user: " + string.Join(", ", userResult.Errors.Select(e => e.Description)));
                        return View();
                    }

                    bool relatedEntityCreated = false;
                    if (user._function == Function.Student)
                    {
                        var student = new Student
                        {
                            UserId = user.Id,
                            RegistrationNumber = _regNumberGenerator.GenerateUniqueRegistrationNumber(),
                        };
                        _context.Students.Add(student);
                        relatedEntityCreated = await _context.SaveChangesAsync() > 0;
                    }
                    else if (user._function == Function.Teacher)
                    {
                        var teacher = new Teacher { UserId = user.Id };
                        _context.Teachers.Add(teacher);
                        relatedEntityCreated = await _context.SaveChangesAsync() > 0;
                    }
                    if (!relatedEntityCreated)
                    {
                        await transaction.RollbackAsync();
                        ModelState.AddModelError(string.Empty, "Error while creating related entity (student/teacher).");
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(string.Empty, "An error occurred while processing the invitation: " + ex.Message);
                    return View();
                }
            }
            return View(model);
        }

        private async Task<bool> SendInvitationEmailAsync(string toEmail, string registrationToken)
        {
            try
            {
                var registrationUrl = Url.Action("Register", "Account", new { token = registrationToken }, protocol: Request.Scheme);
                var subject = "Portal Registration Invitation";
                var body = $"You have been invited to register. Please click the link to complete your registration: <a href=\"{registrationUrl}\">{registrationUrl}</a>";
                var mailManager = new MailManager(_configuration);
                await mailManager.SendEmailAsync(toEmail, subject, body);
                return true;
            }
            catch
            {
                return false;
            }
        }
        [HttpGet]
        public IActionResult InviteUserBulk()
        {
            return View(new BulkInviteViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> InviteUserBulk(BulkInviteViewModel model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();


            if (model.ExcelFile?.Length > 0)
            {
                using var stream = model.ExcelFile.OpenReadStream();
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];

                var rowCount = worksheet.Dimension?.Rows ?? 0;

                for (int row = 2; row <= rowCount + 1; row++)
                {
                    var email = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                    var userTypeStr = worksheet.Cells[row, 2].Value?.ToString()?.Trim();

                    var result = new BulkInviteViewModel.InviteResult
                    {
                        Email = email ?? string.Empty,
                        UserType = Enum.TryParse<Function>(userTypeStr, out var type) ? type : Function.Student
                    };

                    if (string.IsNullOrEmpty(email))
                    {
                        result.IsSuccess = false;
                        result.Message = "Invalid email address";
                        model.ProcessedInvites.Add(result);
                        continue;
                    }

                    // Check if user already exists
                    var existingUser = await _userManager.FindByEmailAsync(email);
                    if (existingUser != null)
                    {
                        result.IsSuccess = false;
                        result.Message = "User already exists";
                        model.ProcessedInvites.Add(result);
                        continue;
                    }

                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        var token = Guid.NewGuid().ToString();
                        var user = new User
                        {
                            Email = result.Email,
                            Name = "temp",
                            Surname = "temp",
                            UserName = result.Email,
                            _accountStatus = Models.User.AccountStatus.PendingActivation,
                            RegistrationToken = token,
                            _function = result.UserType
                        };

                        var userResult = await _userManager.CreateAsync(user, Guid.NewGuid().ToString());
                        if (!userResult.Succeeded)
                        {
                            result.IsSuccess = false;
                            result.Message = "Error while creating user: " + string.Join(", ", userResult.Errors.Select(e => e.Description));
                            await transaction.RollbackAsync();
                            model.ProcessedInvites.Add(result);
                            continue;
                        }

                        // Create related entity
                        bool relatedEntityCreated = false;
                        if (result.UserType == Function.Student)
                        {
                            var student = new Student 
                            { 
                                UserId = user.Id,
                                RegistrationNumber = _regNumberGenerator.GenerateUniqueRegistrationNumber(),
                            };
                            _context.Students.Add(student);
                            relatedEntityCreated = await _context.SaveChangesAsync() > 0;
                        }
                        else if (result.UserType == Function.Teacher)
                        {
                            var teacher = new Teacher { UserId = user.Id };
                            _context.Teachers.Add(teacher);
                            relatedEntityCreated = await _context.SaveChangesAsync() > 0;
                        }
                        if (!relatedEntityCreated)
                        {
                            await transaction.RollbackAsync();
                            result.IsSuccess = false;
                            result.Message = "Error while creating related entity (student/teacher).";
                            model.ProcessedInvites.Add(result);
                            continue;
                        }

                        var invitationSent = await SendInvitationEmailAsync(user.Email, user.RegistrationToken);
                        if (invitationSent)
                        {
                            await transaction.CommitAsync();
                            result.IsSuccess = true;
                            result.Message = "Invitation sent successfully";
                        }
                        else
                        {
                            await transaction.RollbackAsync();
                            result.IsSuccess = false;
                            result.Message = "Error sending invitation email.";
                        }
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        result.IsSuccess = false;
                        result.Message = "Error processing invitation: " + ex.Message;
                    }

                    model.ProcessedInvites.Add(result);
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult TimeTables()
        {
            return View(new TimeTablesViewModel());
        }
    }
}
