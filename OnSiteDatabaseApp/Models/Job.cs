using System.ComponentModel.DataAnnotations;

namespace OnSiteApp.Models;

public class Job
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Customer is required")]
    public string ClientName { get; set; } = "";

    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Service is required")]
    public string Service { get; set; } = "";
    public string Address { get; set; } = "";
    public int? TechnicianId { get; set; }

    public Technician? Technician { get; set; }
    public DateTime ScheduledTime { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Scheduled;
    public List<ChecklistItem> Checklist { get; set; } = new List<ChecklistItem>();

    public int DoneCount
    {
        // Count the checklist items marked complete.
        get
        {
            int count = 0;
            foreach (var item in Checklist)
            {
                if (item.IsCompleted)
                {
                    count++;
                }
            }
            return count;
        }
    }

    public decimal Total
    {
        // Add the billable amounts for completed checklist items.
        get
        {
            decimal total = 0;
            foreach (var item in Checklist)
            {
                if (item.IsCompleted)
                {
                    total = total + item.BillableValue;
                }
            }
            return total;
        }
    }

    public bool IsLate
    {
        // Check whether this unfinished job is past its scheduled time.
        get { return Status != JobStatus.Completed && ScheduledTime < DateTime.Now; }
    }

    public string StatusText
    {
        // Turn the status value into text for display.
        get
        {
            if (Status == JobStatus.EnRoute)
            {
                return "En Route";
            }
            if (Status == JobStatus.InProgress)
            {
                return "In Progress";
            }
            if (Status == JobStatus.Completed)
            {
                return "Completed";
            }
            return "Scheduled";
        }
    }
}
