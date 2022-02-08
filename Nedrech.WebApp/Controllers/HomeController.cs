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

    public IActionResult Index()
    {
        return View(new HomeModel
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
    }

    [HttpPost]
    public async Task<IActionResult> Index(HomeModel homeModel)
    {
        if (homeModel.SelectedIds.Any() && homeModel.Action != HomeModelAction.Refresh)
        {
            Func<IQueryable<ApplicationUser>, IActionResult> action = homeModel.Action switch
            {
                HomeModelAction.Block => Block,
                HomeModelAction.Unblock => Unblock,
                HomeModelAction.Delete => Delete,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            var users = _userManager.Users
                .Where(u => homeModel.SelectedIds.Contains(u.Id));
            
            var result = action(users);

            await _context.SaveChangesAsync();

            return result;
        }

        return Index();
    }
    
    private IActionResult Block(IQueryable<ApplicationUser> users)
    {
        bool containsCurrentUser = false;
        
        foreach (var user in users.Where(u => u.LockoutEnd == null))
        {
            if (user.UserName == User.FindFirstValue(ClaimTypes.Name))
                containsCurrentUser = true;
            
            user.LockoutEnd = DateTimeOffset.MaxValue;
            user.SecurityStamp = Guid.NewGuid().ToString();
        }

        return containsCurrentUser ?
            RedirectToAction("Logout", "Account", new { reason = "You have blocked yourself" }) :
            Index();
    }
    
    private IActionResult Unblock(IQueryable<ApplicationUser> users)
    {
        foreach (var user in users.Where(u => u.LockoutEnd != null))
            user.LockoutEnd = null;

        return Index();
    }

    private IActionResult Delete(IQueryable<ApplicationUser> users)
    {
        bool containsCurrentUser = false;

        foreach (var user in users)
        {
            if (user.UserName == User.FindFirstValue(ClaimTypes.Name))
                containsCurrentUser = true;
            
            _context.Users.Remove(user);
        }

        return containsCurrentUser ?
            RedirectToAction("Logout", "Account", new { reason = "You have deleted your account" })
            : Index();
    }

    public async Task<IActionResult> Add(int count, string pwd = "1")
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