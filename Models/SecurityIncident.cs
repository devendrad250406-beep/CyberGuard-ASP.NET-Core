namespace CyberGuard.Web.Models;

public class SecurityIncident
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string RelatedAlertId { get; set; } = "";
    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "Open";
    public string AssignedTo { get; set; } = "Unassigned";
    public string Notes { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
