namespace OnSiteApp.Models;

public class ChecklistItem
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string TaskLabel { get; set; } = "";
    public bool IsCompleted { get; set; }
    public decimal BillableValue { get; set; }
}
