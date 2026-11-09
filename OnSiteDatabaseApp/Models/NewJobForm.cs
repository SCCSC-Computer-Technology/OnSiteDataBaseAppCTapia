using System.ComponentModel.DataAnnotations;

namespace OnSiteApp.Models;

// Form fields for adding a job, with 4 optional checklist rows.
public class NewJobForm
{
    [Range(1, int.MaxValue, ErrorMessage = "Pick a customer")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Service is required")]
    public string Service { get; set; } = "";

    public int? TechnicianId { get; set; }

    public DateTime ScheduledTime { get; set; } = DateTime.Now;

    public List<ChecklistItem> Items { get; set; } = new List<ChecklistItem>
    {
        new ChecklistItem(),
        new ChecklistItem(),
        new ChecklistItem(),
        new ChecklistItem()
    };
}

public class SchedulesViewModel
{
    public List<Job> Jobs { get; set; } = new List<Job>();
    public List<Customer> Customers { get; set; } = new List<Customer>();
    public List<Technician> Technicians { get; set; } = new List<Technician>();
    public NewJobForm Form { get; set; } = new NewJobForm();
}
