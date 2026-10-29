namespace CyberGuard.Web.Models;

public class SecurityAlert
{
    public string Id { get; set; } = "";
    public string ThreatType { get; set; } = "";
    public string SourceIp { get; set; } = "";
    public string Severity { get; set; } = "Medium";
    public string Status { get; set; } = "Open";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = "";
}
