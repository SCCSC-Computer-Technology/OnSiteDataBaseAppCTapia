using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnSiteApp.Models;
using OnSiteApp.Services;

namespace OnSiteApp.Controllers;

[Authorize(Roles = "Admin")]
public class SchedulesController : Controller
{
    private readonly IJobService _jobs;
    private readonly IUserService _users;

    public SchedulesController(IJobService jobs, IUserService users)
    {
        _jobs = jobs;
        _users = users;
    }

    // Show the schedule and new-job form.
    public IActionResult Index()
    {
        return View(BuildModel(new NewJobForm()));
    }

    [HttpPost, ValidateAntiForgeryToken]
    
    public IActionResult Add(NewJobForm form)
    {
        for (int i = 0; i < form.Items.Count; i++)
        {
            var item = form.Items[i];
            string rowPrefix = $"Items[{i}].";

            // A row without a task is unused, regardless of its price.
            if (string.IsNullOrWhiteSpace(item.TaskLabel))
            {
                var unusedKeys = ModelState.Keys
                    .Where(key => key.StartsWith(rowPrefix, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                foreach (var key in unusedKeys)
                {
                    ModelState.Remove(key);
                }
            }
            else if (item.BillableValue < 0)
            {
                ModelState.AddModelError(rowPrefix + "BillableValue",
                    $"Price for task {i + 1} must be zero or greater.");
            }
        }

        if (!ModelState.IsValid)
        {
            return View("Index", BuildModel(form));
        }

        var customer = _jobs.GetCustomerById(form.CustomerId);
        if (customer == null)
        {
            ModelState.AddModelError("CustomerId", "Pick an existing customer.");
            return View("Index", BuildModel(form));
        }

        var job = new Job();
        job.CustomerId = customer.Id;
        job.ClientName = customer.Name;
        job.Service = form.Service;
        job.TechnicianId = form.TechnicianId;
        job.ScheduledTime = form.ScheduledTime;
        job.Address = customer.Address;

        // skip checklist rows that were left blank
        foreach (var item in form.Items)
        {
            if (!string.IsNullOrWhiteSpace(item.TaskLabel))
            {
                job.Checklist.Add(item);
            }
        }

        _jobs.AddJob(job);

        return RedirectToAction("Index");
    }

    // Load a job and the choices needed to edit it.
    public IActionResult Edit(int id)
    {
        var job = _jobs.GetById(id);
        if (job == null)
        {
            return NotFound();
        }
        ViewBag.Customers = _jobs.GetCustomers();
        ViewBag.Technicians = _users.GetTechnicians();
        return View(job);
    }

    [HttpPost, ValidateAntiForgeryToken]

    public IActionResult Edit(Job job)
    {
        // The customer is picked by id and the name and address come from that customer.
       
        ModelState.Remove("ClientName");
        ModelState.Remove("Address");

        var selectedCustomer = _jobs.GetCustomerById(job.CustomerId);
        if (selectedCustomer == null)
        {
            ModelState.AddModelError("CustomerId", "Pick an existing customer.");
        }

        if (!ModelState.IsValid || selectedCustomer == null)
        {
            ViewBag.Customers = _jobs.GetCustomers();
            ViewBag.Technicians = _users.GetTechnicians();
            return View(job);
        }

        job.ClientName = selectedCustomer.Name;
        job.Address = selectedCustomer.Address;

        _jobs.UpdateJob(job);
        return RedirectToAction("Index");
    }

    [HttpPost, ValidateAntiForgeryToken]


    // Delete a scheduled job.
    public IActionResult Delete(int id)
    {
        _jobs.DeleteJob(id);
        return RedirectToAction("Index");
    }

    
    private SchedulesViewModel BuildModel(NewJobForm form)
    {
        var jobs = _jobs.GetAll();
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

        return new SchedulesViewModel
        {
            Jobs = jobs,
            Customers = _jobs.GetCustomers(),
            Technicians = _users.GetTechnicians(),
            Form = form
        };
    }
}
