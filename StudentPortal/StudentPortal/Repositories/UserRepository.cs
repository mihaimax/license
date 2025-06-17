using Microsoft.EntityFrameworkCore;
using StudentPortal.Interfaces;
using StudentPortal.Models;
using StudentPortal.ViewModels.Admin;
namespace StudentPortal.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public bool Add(User user)
        {
            _context.Users.Add(user);
            return SaveChanges();
        }

        public bool Update(User user)
        {
            _context.Users.Update(user);
            return SaveChanges();
        }

        public bool Delete(User user)
        {
            _context.Users.Remove(user);
            return SaveChanges();
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }

        public string GetIdByEmail(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            return user?.Id.ToString() ?? string.Empty;
        }
        public string GetIdByRegistrationToken(string token)
        {
            var user = _context.Users.FirstOrDefault(u => u.RegistrationToken == token);
            return user?.Id.ToString() ?? string.Empty;
        }
        public async Task<List<UserViewModel>> GetAllViewModelsAsync()
        {
            return await _context.Users
                .Select(s => new UserViewModel
                {
                    UserId = s.Id,
                    UserName = s.UserName,
                    Name = s.Name,
                    Surname = s.Surname,
                    City = s.City,
                    County = s.County,
                    Address = s.Address,
                    CNP = s.CNP,
                    Function = (UserFunction?)s._function,
                    AccountStatus = (AccountStatus?)s._accountStatus
                })
                .ToListAsync();
        }
    }

}
