using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnSiteApp.Models;
using OnSiteApp.Services;

namespace OnSiteApp.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IJobService _jobs;

    
    public HomeController(IJobService jobs)
    {
        _jobs = jobs;
    }

    // 0 means an admin, who can see every job
    
    private int GetTechnicianId()
    {
        if (User.IsInRole("Admin"))
        {
            return 0;
        }
        return Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }

    // Show today's job summary for the current user.
    public IActionResult Index()
    {
        int onTime = 0;
        int late = 0;

        int techId = GetTechnicianId();

        foreach (var job in _jobs.GetAll())
        {
            if (techId != 0 && job.TechnicianId != techId)
            {
                continue;
            }

            if (job.Status != JobStatus.Completed && job.ScheduledTime.Date == DateTime.Today)
            {
                if (job.IsLate)
                {
                    late++;
                }
                else
                {
                    onTime++;
                }
            }
        }

        var model = new HomeViewModel
        {
            OnTime = onTime,
            Late = late
        };
        return View(model);
    }

    
    public IActionResult Analytics()
    {
        TempData["Premium"] = "Premium functionalities Only Available with Premium Purchase.";
        return RedirectToAction("Index");
    }

    // Show a notice that these features are not available 
    public IActionResult AiFunctionalities()
    {
        TempData["Premium"] = "Premium functionalities Only Available with Premium Purchase.";
        return RedirectToAction("Index");
    }

    [AllowAnonymous]
    
    public IActionResult Help()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
