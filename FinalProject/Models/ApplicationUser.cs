using Microsoft.AspNetCore.Identity;

namespace FinalProject.Models;

// Extended IdentityUser with additional user profile fields
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Address { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
