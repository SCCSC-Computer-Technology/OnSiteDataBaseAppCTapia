using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnSiteApp.Models;
using OnSiteApp.Services;

namespace OnSiteApp.Controllers;

[Authorize]
public class JobsController : Controller
{
    private readonly IJobService _jobs;

    // Keep the job service available for job actions.
    public JobsController(IJobService jobs)
    {
        _jobs = jobs;
    }

    // 0 means an admin, who can see every job
    // Return the signed-in technician's ID, or zero for an admin.
    private int GetTechnicianId()
    {
        if (User.IsInRole("Admin"))
        {
            return 0;
        }
        return Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }

    // technicians only get their own jobs
    // Get the jobs the current user is allowed to see.
    private List<Job> MyJobs()
    {
        int techId = GetTechnicianId();
        var myJobs = new List<Job>();
        foreach (var job in _jobs.GetAll())
        {
            if (techId == 0 || job.TechnicianId == techId)
            {
                myJobs.Add(job);
            }
        }
        return myJobs;
    }

    // Find a job the current user is allowed to see.
    private Job? MyJob(int id)
    {
        var job = _jobs.GetById(id);
        if (job == null)
        {
            return null;
        }

        int techId = GetTechnicianId();
        if (techId != 0 && job.TechnicianId != techId)
        {
            return null;
        }
        return job;
    }

    // show todays jobs
    
    public IActionResult Index()
    {
        var jobs = new List<Job>();
        foreach (var job in MyJobs())
        {
            if (job.ScheduledTime.Date == DateTime.Today)
            {
                jobs.Add(job);
            }
        }

        for (int i = 0; i < jobs.Count; i++)
        {
            for (int j = i + 1; j < jobs.Count; j++)
            {
                if (jobs[j].ScheduledTime < jobs[i].ScheduledTime)
                {
                    Job temp = jobs[i];
                    jobs[i] = jobs[j];
                    jobs[j] = temp;
                }
            }
        }
        return View(jobs);
    }

    // Show details for one of the current user's jobs.


    
    public IActionResult Details(int id)
    {
        var job = MyJob(id);
        if (job == null)
        {
            return NotFound();
        }
        return View(job);
    }

    // Navigate button
    [HttpPost, ValidateAntiForgeryToken]
    // Update the job status and open directions to its address.
    public IActionResult Navigate(int id)
    {
        var job = MyJob(id);
        if (job == null)
        {
            return NotFound();
        }

        
        string address = job.Address;
        var customer = _jobs.GetCustomerById(job.CustomerId);
        if (customer != null)
        {
            address = customer.Address;
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            TempData["Error"] = "This job has no valid address. Please check the address first.";
            return RedirectToAction("Details", new { id });
        }


//Redirects to google maps
        _jobs.SetStatus(id, JobStatus.EnRoute);
        return Redirect("https://www.google.com/maps/dir/?api=1&destination=" + Uri.EscapeDataString(address));
    }

    // Arrived button
    [HttpPost, ValidateAntiForgeryToken]


    // Mark the job as in progress.
    public IActionResult Arrive(int id)
    {
        if (MyJob(id) == null)
        {
            return NotFound();
        }

        _jobs.SetStatus(id, JobStatus.InProgress);
        return RedirectToAction("Details", new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]


    // Mark a checklist item complete or incomplete.
    public IActionResult Toggle(int id, int itemId)
    {
        if (MyJob(id) == null)
        {
            return NotFound();
        }

        _jobs.ToggleItem(id, itemId);
        return RedirectToAction("Details", new { id });
    }

  
    [HttpPost, ValidateAntiForgeryToken]

    
    // Complete the job after any required confirmation.
    public IActionResult Complete(int id, bool confirm)
    {
        var job = MyJob(id);
        if (job == null)
        {
            return NotFound();
        }

        if (job.DoneCount == 0 && !confirm)
        {
            TempData["Warning"] = "No checklist items are checked. Complete anyway?";
            return RedirectToAction("Details", new { id });
        }

        _jobs.SetStatus(id, JobStatus.Completed);
        return RedirectToAction("Invoice", new { id });
    }

    // Show completed jobs for the current user.
    public IActionResult Invoices()
    {
        var done = new List<Job>();
        foreach (var job in MyJobs())
        {
            if (job.Status == JobStatus.Completed)
            {
                done.Add(job);
            }
        }
        return View(done);
    }

    // Show an invoice for one of the current user's jobs.
    public IActionResult Invoice(int id)
    {
        var job = MyJob(id);
        if (job == null)
        {
            return NotFound();
        }
        return View(job);
    }
}
