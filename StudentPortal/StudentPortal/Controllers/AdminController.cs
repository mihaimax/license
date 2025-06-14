using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OfficeOpenXml;
using StudentPortal.Classes;
using StudentPortal.Data;
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
                        ModelState.AddModelError(string.Empty, "Error while creating user");
                        return View(model);
                    }
                    switch (model.UserType.ToString())
                    {
                        case "Student":
                            await _userManager.AddToRoleAsync(user, UserRoles.Student);
                            break;
                        case "Teacher":
                            await _userManager.AddToRoleAsync(user, UserRoles.Teacher);
                            break;
                        default:
                            ModelState.AddModelError(string.Empty, "Error while applying role.");
                            break;
                    }
                    await _context.SaveChangesAsync();

                    bool relatedEntityCreated = false;
                    if (user._function == Function.Student)
                    {
                        var student = new Student
                        {
                            UserId = user.Id,
                            RegistrationNumber = _regNumberGenerator.GenerateUniqueRegistrationNumber(),
                            RegisteredOn = DateTime.Now
                        };
                        _context.Students.Add(student);
                        relatedEntityCreated = await _context.SaveChangesAsync() > 0;
                    }
                    else if (user._function == Function.Teacher)
                    {
                        var teacher = new Teacher { 
                            UserId = user.Id,
                            RegisteredOn = DateTime.Now
                        };
                        _context.Teachers.Add(teacher);
                        relatedEntityCreated = await _context.SaveChangesAsync() > 0;
                    }
                    if (!relatedEntityCreated)
                    {
                        await transaction.RollbackAsync();
                        ModelState.AddModelError(string.Empty, "Error while creating related entity (student/teacher).");
                        return View();
                    }
                    var invitationSent = await SendInvitationEmailAsync(user.Email, user.RegistrationToken);
                    if (invitationSent)
                    {
                        await transaction.CommitAsync();
                        TempData["Success"] = "Invitation sent.";
                        return View();
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        ModelState.AddModelError(string.Empty, "An error occurred while sending the invitation");
                        return View();
                    }//Message = "The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_AspNetUserRoles_AspNetUsers_UserId\". The conflict occurred in database \"License\", table \"dbo.AspNetUsers\", column 'Id'.\r\nThe statement has been terminated."
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
        [HttpGet]
        public IActionResult Departments()
        {
            var departments = _context.Departments
                .Select(d => new DepartmentViewModel
                {
                    DepartmentCode = d.DepartmentCode,
                    DepartmentName = d.DepartmentName,
                    DepartmentHeadId = d.DepartmentHeadId,
                    Phone = d.Phone
                })
                .ToList();

            var response = new DepartmentsViewModel
            {
                Department = departments
            };

            return View(response);
        }
        [HttpGet]
        public IActionResult AddDepartment()
        {
            var response = new DepartmentViewModel();
            return View(response);
        }
        [HttpPost]
        public IActionResult AddDepartment(DepartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the Teacher by DepartmentHeadId
                var teacher = _context.Teachers.FirstOrDefault(t => t.TeacherId == model.TeacherId);
                if (teacher == null)
                {
                    ModelState.AddModelError(nameof(model.TeacherId), "Selected department head does not exist.");
                    return View(model);
                }

                var department = new Department
                {
                    DepartmentCode = model.DepartmentCode,
                    DepartmentName = model.DepartmentName,
                    DepartmentHeadId = model.TeacherId, 
                    Phone = model.Phone,

                };

                _context.Departments.Add(department);
                _context.SaveChanges();
                TempData["Success"] = "Department added successfully.";
                return RedirectToAction("Departments");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Subjects()
        {
            var subjects = _context.Subjects
                .Select(s => new SubjectViewModel
                {
                    SubjectCode = s.SubjectCode,
                    SubjectName = s.SubjectName,
                    DepartmentCode = s.DepartmentCode,
                    MinAttendancePercentage = s.MinimumAttendancePercentage,
                    MinExamPercentage = s.MinimumExamPercentage,
                    MinLabPercentage = s.MinimumProjectPercentage,
                    Credits = s.Credits,
                    CourseTeacherId = s.CourseTeacherId,
                    LabTeacherId = s.LabTeacherId
                })
                .ToList();

            var response = new SubjectsViewModel
            {
                Subject = subjects
            };

            return View(response);
        }
        [HttpGet]
        public IActionResult AddSubject()
        {
            var response = new SubjectViewModel();
            return View(response);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSubject(SubjectViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Map the view model to the Subject entity
            var subject = new Subject
            {
                SubjectCode = model.SubjectCode,
                SubjectName = model.SubjectName,
                DepartmentCode = model.DepartmentCode,
                MinimumAttendancePercentage = model.MinAttendancePercentage,
                MinimumExamPercentage = model.MinExamPercentage,
                MinimumProjectPercentage = model.MinLabPercentage,
                Credits = model.Credits,
                CourseTeacherId = model.CourseTeacherId,
                LabTeacherId = model.LabTeacherId
            };

            _context.Subjects.Add(subject);
            _context.SaveChanges();

            TempData["Success"] = "Subject added successfully.";
            return RedirectToAction("Subjects");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportSubjects(IFormFile excelFile)
        {            
            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["Error"] = "Please select a valid Excel file.";
                return RedirectToAction("Subjects");
            }

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                using var stream = excelFile.OpenReadStream();
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];
                
                if (worksheet == null)
                {
                    TempData["Error"] = "No worksheet found in the Excel file.";
                    return RedirectToAction("Subjects");
                }
                
                var rowCount = worksheet.Dimension?.Rows ?? 0;
                int importCount = 0;

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        // Check for empty rows and skip them
                        if (worksheet.Cells[row, 1].Value == null)
                            continue;
                        
                        string subjectCode = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? string.Empty;
                        string subjectName = worksheet.Cells[row, 2].Value?.ToString()?.Trim() ?? string.Empty;
                        string departmentCode = worksheet.Cells[row, 3].Value?.ToString()?.Trim() ?? string.Empty;
                        
                        // Skip empty rows
                        if (string.IsNullOrEmpty(subjectCode) || string.IsNullOrEmpty(subjectName))
                            continue;

                        // Use Value property instead of Text which can be null
                        decimal att = 75.0m;
                        decimal.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out att);
                        
                        decimal exam = 50.0m;
                        decimal.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out exam);
                        
                        decimal lab = 50.0m;
                        decimal.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out lab);
                        
                        int credits = 0;
                        int.TryParse(worksheet.Cells[row, 7].Value?.ToString(), out credits);
                        
                        // Create the subject
                        var subject = new Subject
                        {
                            SubjectCode = subjectCode,
                            SubjectName = subjectName,
                            DepartmentCode = departmentCode,
                            MinimumAttendancePercentage = att,
                            MinimumExamPercentage = exam, 
                            MinimumProjectPercentage = lab,
                            Credits = credits,
                            CourseTeacherId = null, // Set default as null
                            LabTeacherId = null     // Set default as null
                        };

                        // Now handle teacher IDs safely
                        var courseTeacherValue = worksheet.Cells[row, 8].Value;
                        if (courseTeacherValue != null && int.TryParse(courseTeacherValue.ToString(), out int courseTeacherId) && courseTeacherId > 0)
                        {
                            subject.CourseTeacherId = courseTeacherId;
                        }

                        var labTeacherValue = worksheet.Cells[row, 9].Value;
                        if (labTeacherValue != null && int.TryParse(labTeacherValue.ToString(), out int labTeacherId) && labTeacherId > 0)
                        {
                            subject.LabTeacherId = labTeacherId;
                        }

                        _context.Subjects.Add(subject);
                        importCount++;
                    }
                    catch (Exception ex)
                    {
                        // Log or handle individual row errors but keep processing
                        // You might want to collect these errors to display to the user
                        continue;
                    }
                }

                if (importCount > 0)
                {
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    
                    TempData["Success"] = $"{importCount} subjects imported successfully.";
                }
                else
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = "No valid subjects found to import.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Import failed: " + ex.Message;
            }

            return RedirectToAction("Subjects");
        }
        [HttpGet]
        public IActionResult Users()
        {
            var users = _context.Users
                .Select(u => new UserViewModel
                {
                    UserId = u.Id, // <-- Make sure this is included!
                    UserName = u.UserName,
                    Name = u.Name,
                    Surname = u.Surname,
                    City = u.City,
                    County = u.County,
                    Address = u.Address,
                    CNP = u.CNP,
                    Function = (UserFunction?)u._function,
                    AccountStatus = (ViewModels.Admin.AccountStatus?)u._accountStatus
                })
                .ToList();

            var response = new UsersViewModel
            {
                Users = users
            };

            return View(response);
        }
        [HttpGet]
        public IActionResult InviteUser()
        {
            var response = new InviteUserViewModel();
            return View(response);
        }
        [HttpGet]
        public IActionResult UserCard(string userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound();

            var model = new UserViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                Surname = user.Surname,
                City = user.City,
                County = user.County,
                Address = user.Address,
                CNP = user.CNP,
                Function = (UserFunction?)user._function,
                AccountStatus = (ViewModels.Admin.AccountStatus?)user._accountStatus
            };

            return View("UserCard", model);
        }
    }
}
