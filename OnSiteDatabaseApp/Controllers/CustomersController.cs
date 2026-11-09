using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnSiteApp.Models;
using OnSiteApp.Services;

namespace OnSiteApp.Controllers;

[Authorize(Roles = "Admin")]
public class CustomersController : Controller
{
    private readonly IJobService _jobs;

    
    public CustomersController(IJobService jobs)
    {
        _jobs = jobs;
    }

    // Show all customers.
    public IActionResult Index()
    {
        return View(_jobs.GetCustomers());
    }

    [HttpPost, ValidateAntiForgeryToken]


    // Validate and add a customer.
    public IActionResult Add(Customer customer)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", _jobs.GetCustomers());
        }

        _jobs.AddCustomer(customer);
        return RedirectToAction("Index");
    }

    // Load a customer for editing.
    public IActionResult Edit(int id)
    {
        var customer = _jobs.GetCustomerById(id);
        if (customer == null)
        {
            return NotFound();
        }
        return View(customer);
    }

    [HttpPost, ValidateAntiForgeryToken]


    // Save changes to a customer.
    public IActionResult Edit(Customer customer)
    {
        if (!ModelState.IsValid)
        {
            return View(customer);
        }

        _jobs.UpdateCustomer(customer);
        return RedirectToAction("Index");
    }

    [HttpPost, ValidateAntiForgeryToken]

    
    // Delete a customer if the service allows it.
    public IActionResult Delete(int id)
    {
        bool deleted = _jobs.DeleteCustomer(id);
        if (!deleted)
        {
            TempData["Error"] = "This customer has jobs. Delete or reassign those jobs first.";
        }
        return RedirectToAction("Index");
    }
}
