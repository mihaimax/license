﻿using StudentPortal.Models;
using StudentPortal.ViewModels.Admin;
namespace StudentPortal.Interfaces

{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(string id);
        bool Add(User user);
        bool Update(User user);
        bool Delete(User user);
        bool SaveChanges();
        string GetIdByEmail(string email);
        string GetIdByRegistrationToken(string token);
        Task<List<UserViewModel>> GetAllViewModelsAsync();
    }
}
