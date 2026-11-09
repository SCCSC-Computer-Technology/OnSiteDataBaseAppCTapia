namespace OnSiteApp.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId
    {
        // Show the request ID only when one is available.
        get { return !string.IsNullOrEmpty(RequestId); }
    }
}
