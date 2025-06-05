using Microsoft.AspNetCore.Mvc;
namespace StudentPortal.Controllers

{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return RedirectToAction("LogIn");
        }
        public IActionResult LogIn()
        {
            return View(); // Returns the LogIn.cshtml page
        }
    }

}

