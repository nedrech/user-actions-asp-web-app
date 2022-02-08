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

    public HomeController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
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

            var task = Task.Run(async () =>
            {
                foreach (var selectedId in homeModel.SelectedIds)
                {
                    if (User.FindFirstValue(ClaimTypes.NameIdentifier) == selectedId)
                        currentUserContains = true;

                    var user = await _userManager.FindByIdAsync(selectedId);
                    user.LockoutEnd = DateTimeOffset.MaxValue;

                    await _userManager.UpdateSecurityStampAsync(user);
                }
            });
            
            if (currentUserContains)
                return RedirectToAction("Logout", "Account",
                    new { reason = "You have blocked yourself" });

            await task;
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UnblockUsers(HomeModel homeModel)
    {
        foreach (var selectedId in homeModel.SelectedIds)
        {
            var user = await _userManager.FindByIdAsync(selectedId);
            
            await _userManager.SetLockoutEndDateAsync(user, null);
        }
        
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUsers(HomeModel homeModel)
    {
        if (homeModel.SelectedIds.Any())
        {
            bool currentUserContains = false;

            var task = Task.Run(async () =>
            {
                foreach (var selectedId in homeModel.SelectedIds)
                {
                    if (User.FindFirstValue(ClaimTypes.NameIdentifier) == selectedId)
                        currentUserContains = true;

                    var user = await _userManager.FindByIdAsync(selectedId);

                    await _userManager.DeleteAsync(user);
                }
            });

            if (currentUserContains)
                return RedirectToAction("Logout", "Account",
                    new { reason = "You have deleted your account" });

            await task;
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