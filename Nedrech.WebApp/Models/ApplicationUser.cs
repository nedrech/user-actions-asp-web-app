using Microsoft.AspNetCore.Identity;

namespace Nedrech.WebApp.Models;

public class ApplicationUser : IdentityUser
{
    public DateTimeOffset? LastLoginDate { get; set; }

    public DateTimeOffset RegistrationDate { get; set; } = DateTimeOffset.UtcNow;
}