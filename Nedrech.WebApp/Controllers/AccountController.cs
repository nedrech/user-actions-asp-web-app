using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nedrech.WebApp.Models;
using Nedrech.WebApp.Models.ViewModels;

namespace Nedrech.WebApp.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult Login(string? info = null, string? danger = null)
    {
        if (User.Identity is {IsAuthenticated: true})
            return RedirectToAction("Index", "Home");
        return View(new LoginModel{ InfoMessage = info, DangerMessage = danger });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel loginModel)
    {
        ApplicationUser user = await _userManager.FindByNameAsync(loginModel.Username);
        if (user != null)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(user, loginModel.Password,
                false, false);

            if (signInResult.Succeeded)
            {
                user.LastLoginDate = DateTimeOffset.Now;
                
                await _userManager.UpdateAsync(user).ConfigureAwait(false);
                
                return RedirectToAction("Index", "Home");
            }

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("Username", "User is blocked");
            }
            else
            {
                ModelState.AddModelError("Password", "Password is incorrect");
            }
        }
        else
        {
            ModelState.AddModelError("Username", "Couldn't find your username");
        }

        return View(loginModel);
    }

    public IActionResult SignUp()
    {
        if (User.Identity is {IsAuthenticated: true})
            return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(SignupModel signupModel)
    {
        ApplicationUser user = await _userManager.FindByNameAsync(signupModel.Username);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = signupModel.Username,
                Email = signupModel.Email
            };

            var createResult = await _userManager.CreateAsync(user, signupModel.Password);
            if (createResult.Succeeded)
                return RedirectToAction("Login", new { info = "Now you can log in" });
            
            ModelState.AddModelError("", createResult.Errors.First().Description);
        }
        
        ModelState.AddModelError("Username", "Username is already taken");

        return View(signupModel);
    }

    [Authorize]
    public async Task<IActionResult> Logout(string? reason = null) {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", new { danger = reason });
    }
}