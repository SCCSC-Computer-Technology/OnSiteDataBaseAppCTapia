namespace OnSiteApp.Models;

public class Technician
{
    public int Id { get; set; }
    public int TechNumber { get; set; }
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
}

public class AdminUser
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string PasswordHash { get; set; } = "";
}
