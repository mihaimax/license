using StudentPortal.Models;
namespace StudentPortal.Interfaces

{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);

        bool Add(User user);
        bool Update(User user);
        bool Delete(User user);
        bool SaveChanges();
    }
}
