using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnSiteApp.Models;
using OnSiteApp.Services;

namespace OnSiteApp.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _users;

   
    public AccountController(IUserService users)
    {
        _users = users;
    }

    [AllowAnonymous]


    // Show the sign-in form.
    public IActionResult Login()
    {
         if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        return View(new LoginForm());
    }

    [AllowAnonymous]
    [HttpPost, ValidateAntiForgeryToken]


    // Check the credentials  

    
    public async Task<IActionResult> Login(LoginForm form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var claims = new List<Claim>();

        if (form.IsAdmin)
        {
            var admin = _users.FindAdmin(form.UserName.Trim());
            if (admin == null || !PasswordHelper.Check(admin.PasswordHash, form.Password))
            {
                ModelState.AddModelError("", "Wrong admin name or password.");
                return View(form);
            }

            claims.Add(new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, admin.Name));
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            claims.Add(new Claim("Label", "Admin: " + admin.Name));
        }
        else
        {
            Technician? technician = null;
            int techNumber = 0;
            if (int.TryParse(form.UserName.Trim(), out techNumber))
            {
                technician = _users.FindTechnician(techNumber);
            }

            if (technician == null || !PasswordHelper.Check(technician.PasswordHash, form.Password))
            {
                ModelState.AddModelError("", "Wrong technician number or password.");
                return View(form);
            }

            claims.Add(new Claim(ClaimTypes.NameIdentifier, technician.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, technician.Name));
            claims.Add(new Claim(ClaimTypes.Role, "Technician"));
            claims.Add(new Claim("Label", "Tech: " + technician.TechNumber));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpPost, ValidateAntiForgeryToken]


    // Sign  user out.
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [Authorize]


    // Show the password Change form
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordForm());
    }

    [Authorize]
    [HttpPost, ValidateAntiForgeryToken]

    
    // Saves new passowrd(checks)
    public IActionResult ChangePassword(ChangePasswordForm form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        string currentHash = "";

        if (User.IsInRole("Admin"))
        {
            var admin = _users.GetAdminById(userId);
            if (admin != null)
            {
                currentHash = admin.PasswordHash;
            }
        }
        else
        {
            var technician = _users.GetTechnicianById(userId);
            if (technician != null)
            {
                currentHash = technician.PasswordHash;
            }
        }

        if (currentHash == "" || !PasswordHelper.Check(currentHash, form.CurrentPassword))
        {
            ModelState.AddModelError("CurrentPassword", "Your current password is wrong.");
            return View(form);
        }

        string newHash = PasswordHelper.Hash(form.NewPassword);
        if (User.IsInRole("Admin"))
        {
            _users.SetAdminPassword(userId, newHash);
        }
        else
        {
            _users.SetTechnicianPassword(userId, newHash);
        }

        TempData["Message"] = "Your password was changed.";
        return RedirectToAction("Index", "Home");
    }
}
