using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnSiteApp.Models;
using OnSiteApp.Services;

namespace OnSiteApp.Controllers;

[Authorize(Roles = "Admin")]
public class AdministratorController : Controller
{
    private readonly IUserService _users;
    private readonly IJobService _jobs;
    


    public AdministratorController(IUserService users, IJobService jobs)
    {
        _users = users;
        _jobs = jobs;
    }

    // Show the administrator page.
    public IActionResult Index()
    {
        return View(BuildModel(new TechnicianForm(), new AdminForm()));
    }

    [HttpPost, ValidateAntiForgeryToken]


  
  //Adds Tech. 

    public IActionResult AddTechnician(TechnicianForm form)
    {
        if (_users.FindTechnician(form.TechNumber) != null)
        {
            ModelState.AddModelError("TechNumber", "That technician number is already used.");
        }

        if (!ModelState.IsValid)
        {
            return View("Index", BuildModel(form, new AdminForm()));
        }

        var technician = new Technician();
        technician.TechNumber = form.TechNumber;
        technician.Name = form.Name;
        technician.Phone = form.Phone;
        technician.Email = form.Email;
        technician.PasswordHash = PasswordHelper.Hash(form.Password);
        _users.AddTechnician(technician);

        return RedirectToAction("Index");
    }

    [HttpPost, ValidateAntiForgeryToken]


    //Deletes TEch
    public IActionResult DeleteTechnician(int id)
    {
        _jobs.UnassignTechnician(id);
        _users.DeleteTechnician(id);
        return RedirectToAction("Index");
    }

    [HttpPost, ValidateAntiForgeryToken]


   //Adds Admin.
    public IActionResult AddAdmin(AdminForm form)
    {
        if (_users.FindAdmin(form.Name.Trim()) != null)
        {
            ModelState.AddModelError("Name", "That administrator already exists.");
        }

        if (!ModelState.IsValid)
        {
            return View("Index", BuildModel(new TechnicianForm(), form));
        }

        var admin = new AdminUser();
        admin.Name = form.Name.Trim();
        admin.PasswordHash = PasswordHelper.Hash(form.Password);
        _users.AddAdmin(admin);

        return RedirectToAction("Index");
    }

    [HttpPost, ValidateAntiForgeryToken]


    // Set a new password for a technician.
    public IActionResult ResetTechnicianPassword(int id, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 4)
        {
            TempData["Error"] = "The new password must be at least 4 characters.";
            return RedirectToAction("Index");
        }

        var technician = _users.GetTechnicianById(id);
        if (technician == null)
        {
            return NotFound();
        }

        _users.SetTechnicianPassword(id, PasswordHelper.Hash(newPassword));
        TempData["Message"] = "Password reset for technician " + technician.TechNumber + ".";
        return RedirectToAction("Index");
    }

    [HttpPost, ValidateAntiForgeryToken]

    
    // Set a new password for an administrator.
    public IActionResult ResetAdminPassword(int id, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 4)
        {
            TempData["Error"] = "The new password must be at least 4 characters.";
            return RedirectToAction("Index");
        }

        var admin = _users.GetAdminById(id);
        if (admin == null)
        {
            return NotFound();
        }

        _users.SetAdminPassword(id, PasswordHelper.Hash(newPassword));
        TempData["Message"] = "Password reset for administrator " + admin.Name + ".";
        return RedirectToAction("Index");
    }

    // Gather the data needed by the administrator page.
    private AdministratorViewModel BuildModel(TechnicianForm techForm, AdminForm adminForm)
    {
        return new AdministratorViewModel
        {
            Technicians = _users.GetTechnicians(),
            Admins = _users.GetAdmins(),
            TechForm = techForm,
            AdminForm = adminForm
        };
    }
}
