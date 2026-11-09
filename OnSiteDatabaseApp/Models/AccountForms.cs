using System.ComponentModel.DataAnnotations;

namespace OnSiteApp.Models;

public class LoginForm
{
    public bool IsAdmin { get; set; }

    [Required(ErrorMessage = "Enter your technician number or admin name")]
    public string UserName { get; set; } = "";

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = "";
}

public class ChangePasswordForm
{
    [Required(ErrorMessage = "Enter your current password")]
    public string CurrentPassword { get; set; } = "";

    [Required(ErrorMessage = "Enter a new password")]
    [MinLength(4, ErrorMessage = "The new password must be at least 4 characters")]
    public string NewPassword { get; set; } = "";

    [Compare("NewPassword", ErrorMessage = "The new passwords do not match")]
    public string ConfirmPassword { get; set; } = "";
}
