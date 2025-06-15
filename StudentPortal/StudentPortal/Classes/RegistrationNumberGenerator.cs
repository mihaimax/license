using StudentPortal.Models;

namespace StudentPortal.Classes
{
    public class RegistrationNumberGenerator
    {
        private readonly ApplicationDbContext _context;
        private readonly Random _random = new();

        public RegistrationNumberGenerator(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GenerateUniqueRegistrationNumber()
        {
            string regNumber;
            do
            {
                regNumber = _random.Next(10000000, 100000000).ToString();
            }
            while (_context.Students.Any(s => s.RegistrationNumber == regNumber));
            return regNumber;
        }
    }
}
