using System.ComponentModel.DataAnnotations;

namespace OnSiteApp.Models;

public class Customer
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = "";

    public string Phone { get; set; } = "";
    public string Address { get; set; } = "";
}
