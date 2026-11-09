using Microsoft.EntityFrameworkCore;
using OnSiteApp.Data;
using OnSiteApp.Models;

namespace OnSiteApp.Services;

public class JobService : IJobService
{
    private readonly OnSiteDatabase _context;

    // Keep the database context available for job and customer data.
    public JobService(OnSiteDatabase context)
    {
        _context = context;
    }

    // Get every job with its checklist items.
    public List<Job> GetAll()
    {
        return _context.Jobs.Include(job => job.Checklist).ToList();
    }

  
    public Job? GetById(int id)
    {
        return _context.Jobs.Include(job => job.Checklist).FirstOrDefault(job => job.Id == id);
    }

    // Update a job's status if it exists.
    public void SetStatus(int id, JobStatus status)
    {
        var job = _context.Jobs.Find(id);
        if (job != null)
        {
            job.Status = status;
            _context.SaveChanges();
        }
    }

    // Switch a checklist item between complete and incomplete.
    public void ToggleItem(int jobId, int itemId)
    {
        var item = _context.ChecklistItems.FirstOrDefault(item => item.JobId == jobId && item.Id == itemId);
        if (item != null)
        {
            item.IsCompleted = !item.IsCompleted;
            _context.SaveChanges();
        }
    }

    // Save a job and its checklist items.
    public void AddJob(Job job)
    {
        _context.Jobs.Add(job);
        _context.SaveChanges();
    }

    // Get all customers.
    public List<Customer> GetCustomers()
    {
        return _context.Customers.ToList();
    }

    // Find a customer by database ID.
    public Customer? GetCustomerById(int id)
    {
        return _context.Customers.Find(id);
    }

    // Save a new customer.
    public void AddCustomer(Customer customer)
    {
        _context.Customers.Add(customer);
        _context.SaveChanges();
    }

    // Update a customer's details and refresh their job snapshots.
    public void UpdateCustomer(Customer customer)
    {
        _context.Customers.Update(customer);
        _context.SaveChanges();

        var jobs = _context.Jobs.Where(job => job.CustomerId == customer.Id).ToList();
        foreach (var job in jobs)
        {
            job.ClientName = customer.Name;
            job.Address = customer.Address;
        }
        _context.SaveChanges();
    }

    // Delete a customer only when no jobs refer to them.
    public bool DeleteCustomer(int id)
    {
        bool hasJobs = _context.Jobs.Any(job => job.CustomerId == id);
        if (hasJobs)
        {
            return false;
        }

        var customer = _context.Customers.Find(id);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }
        return true;
    }

    // Update the editable details of an existing job.
    public void UpdateJob(Job job)
    {
        var existing = _context.Jobs.Include(item => item.Checklist).FirstOrDefault(item => item.Id == job.Id);
        if (existing != null)
        {
            existing.CustomerId = job.CustomerId;
            existing.ClientName = job.ClientName;
            existing.Service = job.Service;
            existing.Address = job.Address;
            existing.TechnicianId = job.TechnicianId;
            existing.ScheduledTime = job.ScheduledTime;
            _context.SaveChanges();
        }
    }

    // Remove a technician assignment from all their jobs.
    public void UnassignTechnician(int technicianId)
    {
        var jobs = _context.Jobs.Where(job => job.TechnicianId == technicianId).ToList();
        foreach (var job in jobs)
        {
            job.TechnicianId = null;
        }
        _context.SaveChanges();
    }

    // Delete a job if it exists.
    public void DeleteJob(int id)
    {
        var job = _context.Jobs.Find(id);
        if (job != null)
        {
            _context.Jobs.Remove(job);
            _context.SaveChanges();
        }
    }
}