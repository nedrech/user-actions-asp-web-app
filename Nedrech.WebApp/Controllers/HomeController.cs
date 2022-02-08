using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nedrech.WebApp.Models;
using Nedrech.WebApp.Models.ViewModels;

namespace Nedrech.WebApp.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppIdentityDbContext _context;

    public HomeController(UserManager<ApplicationUser> userManager, AppIdentityDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public IActionResult Index() =>
        View(new HomeModel
        {
            CurrentUsername = User.FindFirstValue(ClaimTypes.Name),
            Users = _userManager.Users
                .OrderByDescending(u => u.RegistrationDate)
                .Select(u => new ApplicationUser
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    RegistrationDate = u.RegistrationDate,
                    LastLoginDate = u.LastLoginDate,
                    LockoutEnd = u.LockoutEnd
                })
        });

    [HttpPost]
    public async Task<IActionResult> BlockUsers(HomeModel homeModel)
    {
        if (homeModel.SelectedIds.Any())
        {
            bool currentUserContains = false;

            var users = _userManager.Users
                .Where(u => homeModel.SelectedIds.Contains(u.Id));

            foreach (var user in users)
            {
                if (User.FindFirstValue(ClaimTypes.NameIdentifier) == user.Id)
                    currentUserContains = true;
                
                user.LockoutEnd = DateTimeOffset.MaxValue;
                user.SecurityStamp = Guid.NewGuid().ToString();
            }

            await _context.SaveChangesAsync();

            if (currentUserContains)
                return RedirectToAction("Logout", "Account",
                    new { reason = "You have blocked yourself" });
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UnblockUsers(HomeModel homeModel)
    {
        if (homeModel.SelectedIds.Any())
        {
            var users = _userManager.Users
                .Where(u => homeModel.SelectedIds.Contains(u.Id));
            
            foreach (var user in users)
            {
                user.LockoutEnd = null;
            }

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUsers(HomeModel homeModel)
    {
        if (homeModel.SelectedIds.Any())
        {
            bool currentUserContains = false;
            
            var users = _userManager.Users
                .Where(u => homeModel.SelectedIds.Contains(u.Id));

            foreach (var user in users)
            {
                if (User.FindFirstValue(ClaimTypes.NameIdentifier) == user.Id)
                    currentUserContains = true;

                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            
            if (currentUserContains)
                return RedirectToAction("Login", "Account",
                    new { danger = "You have deleted your account" });
        }
        
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> AddUsers(int count, string pwd = "1")
    {
        var httpClient = new HttpClient();
        
        for (int i = 0; i < count; i++)
        {
            var response = await httpClient.GetStringAsync("https://randomuser.me/api/");

            dynamic? jObject = JsonSerializer.Deserialize(response, new
            {
                results = new []
                {
                    new
                    {
                        login = new
                        {
                            username = string.Empty
                        },
                        email = string.Empty
                    }
                }
            }.GetType());

            var result = jObject?.results[0];
            ApplicationUser user = await _userManager.FindByNameAsync(result?.login.username);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = result?.login.username,
                    Email = result?.email
                };
                
                await _userManager.CreateAsync(user, pwd);
            }
            else
            {
                i--;
            }
        }

        return RedirectToAction("Index");
    }
}