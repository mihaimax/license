using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StudentPortal.Models;

namespace StudentPortal.Data
{
    public class seed
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure the Admin role exists
            var adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }
            var StudentRole = "Student";
            if (!await roleManager.RoleExistsAsync(StudentRole))
            {
                await roleManager.CreateAsync(new IdentityRole(StudentRole));
            }
            var TeacherRole = "Teacher";
            if (!await roleManager.RoleExistsAsync(TeacherRole))
            {
                await roleManager.CreateAsync(new IdentityRole(TeacherRole));
            }

            // Create the admin user if it doesn't exist
            var adminEmail = "admin@licenta.ro";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = "Admin",
                    Surname = "User",
                    _function = User.Function.Admin,
                    _accountStatus = User.AccountStatus.Active
                };

                var result = await userManager.CreateAsync(user, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, adminRole);
                }
            }
        }
    }
}
