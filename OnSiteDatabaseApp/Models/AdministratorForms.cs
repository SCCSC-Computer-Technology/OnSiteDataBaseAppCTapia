using System.ComponentModel.DataAnnotations;

namespace OnSiteApp.Models;

public class TechnicianForm
{
    [Range(1, 999999, ErrorMessage = "Technician number must be between 1 and 999999")]
    public int TechNumber { get; set; }

    [Required(ErrorMessage = "Technician name is required")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Technician phone is required")]
    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "Technician email is required")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Technician password is required")]
    public string Password { get; set; } = "";

}

public class AdminForm
{
    [Required(ErrorMessage = "Administrator name is required")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Administrator password is required")]
    public string Password { get; set; } = "";

}

public class AdministratorViewModel
{
    public List<Technician> Technicians { get; set; } = new List<Technician>();
    public List<AdminUser> Admins { get; set; } = new List<AdminUser>();
    public TechnicianForm TechForm { get; set; } = new TechnicianForm();
    public AdminForm AdminForm { get; set; } = new AdminForm();
}
