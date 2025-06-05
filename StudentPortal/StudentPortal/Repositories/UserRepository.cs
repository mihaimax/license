using StudentPortal.Interfaces;
using StudentPortal.Models;
using System.Data.Entity;
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
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
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
    }

}
