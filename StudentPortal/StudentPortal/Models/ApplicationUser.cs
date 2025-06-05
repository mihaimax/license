using Microsoft.AspNetCore.Identity;
namespace StudentPortal.Models

{ }
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
