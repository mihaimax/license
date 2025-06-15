using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using StudentPortal.Classes;
using StudentPortal.Data;
using StudentPortal.Interfaces;
using StudentPortal.Models;
using StudentPortal.ViewModels.Admin;
using StudentPortal.Classes;
using static StudentPortal.Models.User;
using static StudentPortal.ViewModels.Admin.TimeTableViewModel;

namespace StudentPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ITimeTableRepository _timeTableRepository;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RegistrationNumberGenerator _regNumberGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            IStudentRepository studentRepository,
            ITeacherRepository teacherRepository,
            IDepartmentRepository departmentRepository,
            ISubjectRepository subjectRepository,
            ITimeTableRepository timeTableRepository,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            RegistrationNumberGenerator regNumberGenerator)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _departmentRepository = departmentRepository;
            _subjectRepository = subjectRepository;
            _timeTableRepository = timeTableRepository;
            _userManager = userManager;
            _signInManager = signInManager;
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
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "A user with this email already exists.");
                    return View(model);
                }
                await using var transaction = await _unitOfWork.BeginTransactionAsync();
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
                    await _unitOfWork.SaveChangesAsync();

                    bool relatedEntityCreated = false;
                    if (user._function == Function.Student)
                    {
                        var student = new Student
                        {
                            UserId = user.Id,
                            RegistrationNumber = _regNumberGenerator.GenerateUniqueRegistrationNumber(),
                            RegisteredOn = DateTime.Now
                        };
                        relatedEntityCreated = await _unitOfWork.Students.AddAsync(student);
                        relatedEntityCreated = await _unitOfWork.SaveChangesAsync() > 0;
                    }
                    else if (user._function == Function.Teacher)
                    {
                        var teacher = new Teacher
                        {
                            UserId = user.Id,
                            RegisteredOn = DateTime.Now
                        };
                        relatedEntityCreated = await _unitOfWork.Teachers.AddAsync(teacher);
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
                        UserType = Enum.TryParse<User.Function>(userTypeStr, out var type) ? type : Models.User.Function.Student
                    };

                    if (string.IsNullOrEmpty(email))
                    {
                        result.IsSuccess = false;
                        result.Message = "Invalid email address";
                        model.ProcessedInvites.Add(result);
                        continue;
                    }

                    var existingUser = await _userManager.FindByEmailAsync(email);
                    if (existingUser != null)
                    {
                        result.IsSuccess = false;
                        result.Message = "User already exists";
                        model.ProcessedInvites.Add(result);
                        continue;
                    }

                    await using var transaction = await _unitOfWork.BeginTransactionAsync();
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

                        bool relatedEntityCreated = false;
                        if (result.UserType == Models.User.Function.Student)
                        {
                            var student = new Student
                            {
                                UserId = user.Id,
                                RegistrationNumber = _regNumberGenerator.GenerateUniqueRegistrationNumber(),
                            };
                            await _unitOfWork.Students.AddAsync(student);
                            relatedEntityCreated = await _unitOfWork.SaveChangesAsync() > 0;
                        }
                        else if (result.UserType == Models.User.Function.Teacher)
                        {
                            var teacher = new Teacher { UserId = user.Id };
                            await _unitOfWork.Teachers.AddAsync(teacher);
                            relatedEntityCreated = await _unitOfWork.SaveChangesAsync() > 0;
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
        public async Task<IActionResult> TimeTables()
        {
            var timeTables = await _timeTableRepository.GetAllViewModelsAsync();
            return View(new TimeTablesViewModel { TimeTable = timeTables });
        }

        [HttpGet]
        public async Task<IActionResult> Departments()
        {
            var departments = await _departmentRepository.GetAllViewModelsAsync();
            var response = new DepartmentsViewModel { Department = departments };
            return View(response);
        }

        [HttpGet]
        public IActionResult AddDepartment()
        {
            return View(new DepartmentViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddDepartment(DepartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var teacher = await _teacherRepository.GetTeacherByIdAsync(model.TeacherId);
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

                await _departmentRepository.AddAsync(department);
                await _departmentRepository.SaveChangesAsync();
                TempData["Success"] = "Department added successfully.";
                return RedirectToAction("Departments");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Subjects()
        {
            var subjects = await _subjectRepository.GetAllViewModelsAsync();
            var response = new SubjectsViewModel { Subject = subjects };
            return View(response);
        }

        [HttpGet]
        public IActionResult AddSubject()
        {
            return View(new SubjectViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSubject(SubjectViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

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

            await _subjectRepository.AddAsync(subject);
            await _subjectRepository.SaveChangesAsync();

            TempData["Success"] = "Subject added successfully.";
            return RedirectToAction("Subjects");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportSubjects(SubjectViewModel model)
        {
            model.ProcessedRows = new List<SubjectViewModel.RowResult>();

            if (model.ExcelFile == null || model.ExcelFile.Length == 0)
            {
                TempData["Error"] = "Please select a valid Excel file.";
                return RedirectToAction("Subjects");
            }

            try
            {
                await using var transaction = await _unitOfWork.BeginTransactionAsync();
                using var stream = model.ExcelFile.OpenReadStream();
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
                    var result = new SubjectViewModel.RowResult { RowNumber = row };
                    try
                    {
                        string subjectCode = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? string.Empty;
                        string subjectName = worksheet.Cells[row, 2].Value?.ToString()?.Trim() ?? string.Empty;
                        string departmentCode = worksheet.Cells[row, 3].Value?.ToString()?.Trim() ?? string.Empty;

                        if (string.IsNullOrEmpty(subjectCode) || string.IsNullOrEmpty(subjectName))
                        {
                            result.IsSuccess = false;
                            result.Message = "Subject code and name are required.";
                            model.ProcessedRows.Add(result);
                            continue;
                        }

                        var department = await _unitOfWork.Departments.GetByCodeAsync(departmentCode);
                        if (department == null)
                        {
                            result.IsSuccess = false;
                            result.Message = $"Department code '{departmentCode}' does not exist.";
                            model.ProcessedRows.Add(result);
                            continue;
                        }

                        decimal att = 75.0m;
                        decimal.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out att);

                        decimal exam = 50.0m;
                        decimal.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out exam);

                        decimal lab = 50.0m;
                        decimal.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out lab);

                        int credits = 0;
                        int.TryParse(worksheet.Cells[row, 7].Value?.ToString(), out credits);

                        int? courseTeacherId = null;
                        var courseTeacherValue = worksheet.Cells[row, 8].Value;
                        if (courseTeacherValue != null && int.TryParse(courseTeacherValue.ToString(), out int parsedCourseTeacherId) && parsedCourseTeacherId > 0)
                        {
                            var courseTeacher = await _unitOfWork.Teachers.GetTeacherByIdAsync(parsedCourseTeacherId);
                            if (courseTeacher == null)
                            {
                                result.IsSuccess = false;
                                result.Message = $"Course Teacher ID '{parsedCourseTeacherId}' does not exist.";
                                model.ProcessedRows.Add(result);
                                continue;
                            }
                            courseTeacherId = parsedCourseTeacherId;
                        }

                        int? labTeacherId = null;
                        var labTeacherValue = worksheet.Cells[row, 9].Value;
                        if (labTeacherValue != null && int.TryParse(labTeacherValue.ToString(), out int parsedLabTeacherId) && parsedLabTeacherId > 0)
                        {
                            var labTeacher = await _unitOfWork.Teachers.GetTeacherByIdAsync(parsedLabTeacherId);
                            if (labTeacher == null)
                            {
                                result.IsSuccess = false;
                                result.Message = $"Lab Teacher ID '{parsedLabTeacherId}' does not exist.";
                                model.ProcessedRows.Add(result);
                                continue;
                            }
                            labTeacherId = parsedLabTeacherId;
                        }

                        if (await _unitOfWork.Subjects.ExistsAsync(subjectCode))
                        {
                            result.IsSuccess = false;
                            result.Message = $"Subject code '{subjectCode}' already exists.";
                            model.ProcessedRows.Add(result);
                            continue;
                        }

                        var subject = new Subject
                        {
                            SubjectCode = subjectCode,
                            SubjectName = subjectName,
                            DepartmentCode = departmentCode,
                            MinimumAttendancePercentage = att,
                            MinimumExamPercentage = exam,
                            MinimumProjectPercentage = lab,
                            Credits = credits,
                            CourseTeacherId = courseTeacherId,
                            LabTeacherId = labTeacherId
                        };

                        await _unitOfWork.Subjects.AddAsync(subject);
                        importCount++;
                        result.IsSuccess = true;
                        result.Message = "Imported successfully.";
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = $"Error: {ex.Message}";
                    }
                    model.ProcessedRows.Add(result);
                }

                if (importCount > 0)
                {
                    await _unitOfWork.SaveChangesAsync();
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

            var subjectsViewModel = new SubjectsViewModel
            {
                Subject = await _unitOfWork.Subjects.GetAllViewModelsAsync(),
                ImportResults = model.ProcessedRows
            };

            return View("Subjects", subjectsViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var userViewModels = users.Select(user => new UserViewModel
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
            }).ToList();

            var response = new UsersViewModel { Users = userViewModels };
            return View(response);
        }

        [HttpGet]
        public IActionResult InviteUser()
        {
            return View(new InviteUserViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> UserCard(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
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

        [HttpGet]
        public async Task<IActionResult> DepartmentCard(string departmentCode)
        {
            if (string.IsNullOrEmpty(departmentCode))
                return NotFound();

            var department = await _departmentRepository.GetByCodeAsync(departmentCode);
            if (department == null)
                return NotFound();

            string? departmentHeadName = null;
            if (department.DepartmentHeadId.HasValue)
            {
                var head = await _teacherRepository.GetTeacherByIdAsync(department.DepartmentHeadId.Value);
                if (head != null)
                    departmentHeadName = head.User?.Name + " " + head.User?.Surname;
            }

            var model = new DepartmentViewModel
            {
                DepartmentCode = department.DepartmentCode,
                DepartmentName = department.DepartmentName,
                DepartmentHead = departmentHeadName,
                DepartmentHeadId = department.DepartmentHeadId,
                Phone = department.Phone,
                TeacherId = department.DepartmentHeadId ?? 0
            };

            return View("DepartmentCard", model);
        }

        [HttpGet]
        public async Task<IActionResult> SubjectCard(string subjectCode)
        {
            if (string.IsNullOrEmpty(subjectCode))
                return NotFound();

            var subject = await _subjectRepository.GetByCodeAsync(subjectCode);
            if (subject == null)
                return NotFound();

            var model = new SubjectViewModel
            {
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                DepartmentCode = subject.DepartmentCode,
                MinAttendancePercentage = subject.MinimumAttendancePercentage,
                MinExamPercentage = subject.MinimumExamPercentage,
                MinLabPercentage = subject.MinimumProjectPercentage,
                Credits = subject.Credits,
                CourseTeacherId = subject.CourseTeacherId,
                LabTeacherId = subject.LabTeacherId
            };

            return View("SubjectCard", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportTimeTable(TimeTableViewModel model)
        {
            model.ProcessedRows = new List<TimeTableViewModel.RowResult>();

            if (model.ExcelFile == null || model.ExcelFile.Length == 0)
            {
                TempData["Error"] = "Please select a valid Excel file.";
                return RedirectToAction("TimeTables");
            }

            try
            {
                await using var transaction = await _unitOfWork.BeginTransactionAsync();
                using var stream = model.ExcelFile.OpenReadStream();
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];

                if (worksheet == null)
                {
                    TempData["Error"] = "No worksheet found in the Excel file.";
                    return RedirectToAction("TimeTables");
                }

                var rowCount = worksheet.Dimension?.Rows ?? 0;
                int importCount = 0;

                for (int row = 2; row <= rowCount; row++)
                {
                    var result = new TimeTableViewModel.RowResult { RowNumber = row };
                    try
                    {
                        string departmentCode = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? string.Empty;
                        string subjectCode = worksheet.Cells[row, 2].Value?.ToString()?.Trim() ?? string.Empty;
                        bool isLab = bool.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out var lab) && lab;
                        int? labTeacherId = int.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out var lt) ? lt : (int?)null;
                        string weekday = worksheet.Cells[row, 5].Value?.ToString()?.Trim() ?? string.Empty;
                        string startTimeStr = worksheet.Cells[row, 6].Value?.ToString()?.Trim() ?? string.Empty;
                        string endTimeStr = worksheet.Cells[row, 7].Value?.ToString()?.Trim() ?? string.Empty;
                        string specialization = worksheet.Cells[row, 8].Value?.ToString()?.Trim() ?? string.Empty;
                        int year = int.TryParse(worksheet.Cells[row, 9].Value?.ToString(), out var y) ? y : 0;
                        int semester = int.TryParse(worksheet.Cells[row, 10].Value?.ToString(), out var s) ? s : 0;

                        if (string.IsNullOrEmpty(departmentCode) || string.IsNullOrEmpty(subjectCode) ||
                            string.IsNullOrEmpty(weekday) || string.IsNullOrEmpty(startTimeStr) || string.IsNullOrEmpty(endTimeStr))
                        {
                            result.IsSuccess = false;
                            result.Message = "Required fields are missing.";
                            model.ProcessedRows.Add(result);
                            continue;
                        }

                        object startCell = worksheet.Cells[row, 6].Value;
                        TimeOnly startTime;
                        bool startTimeParsed = false;

                        if (startCell is DateTime dt)
                        {
                            startTime = TimeOnly.FromDateTime(dt);
                            startTimeParsed = true;
                        }
                        else if (double.TryParse(startCell?.ToString(), out double oaDate))
                        {
                            var dateTime = DateTime.FromOADate(oaDate);
                            startTime = TimeOnly.FromDateTime(dateTime);
                            startTimeParsed = true;
                        }
                        else if (TimeOnly.TryParse(startCell?.ToString(), out var t))
                        {
                            startTime = t;
                            startTimeParsed = true;
                        }
                        else
                        {
                            result.Message = "Invalid start time format. Please use hh:mm (e.g., 08:30).";
                            model.ProcessedRows.Add(result);
                            continue;
                        }

                        object endCell = worksheet.Cells[row, 7].Value;
                        TimeOnly endTime;
                        bool endTimeParsed = false;

                        if (endCell is DateTime dtEnd)
                        {
                            endTime = TimeOnly.FromDateTime(dtEnd);
                            endTimeParsed = true;
                        }
                        else if (double.TryParse(endCell?.ToString(), out double oaDateEnd))
                        {
                            var dateTime = DateTime.FromOADate(oaDateEnd);
                            endTime = TimeOnly.FromDateTime(dateTime);
                            endTimeParsed = true;
                        }
                        else if (TimeOnly.TryParse(endCell?.ToString(), out var tEnd))
                        {
                            endTime = tEnd;
                            endTimeParsed = true;
                        }
                        else
                        {
                            result.Message = "Invalid end time format. Please use hh:mm (e.g., 08:30).";
                            model.ProcessedRows.Add(result);
                            continue;
                        }

                        var timeTable = new TimeTable
                        {
                            DepartmentCode = departmentCode,
                            Year = year,
                            Semester = semester,
                            SubjectCode = subjectCode,
                            Weekday = weekday,
                            StartTime = startTime,
                            EndTime = endTime,
                            IsLab = isLab,
                            LabTeacherId = labTeacherId,
                            Specialization = specialization
                        };

                        await _unitOfWork.TimeTables.AddAsync(timeTable);
                        importCount++;
                        result.IsSuccess = true;
                        result.Message = "Imported successfully.";
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.Message = $"Error: {ex.Message}";
                    }
                    model.ProcessedRows.Add(result);
                }

                if (importCount > 0)
                {
                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["Success"] = $"{importCount} timetable entries imported successfully.";
                }
                else
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = "No valid timetable entries found to import.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Import failed: " + ex.Message;
            }

            var timeTables = await _unitOfWork.TimeTables.GetAllViewModelsAsync();
            var viewModel = new TimeTablesViewModel
            {
                TimeTable = timeTables,
                ImportResults = model.ProcessedRows
            };

            return View("TimeTables", viewModel);
        }
    }
}