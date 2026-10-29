namespace CyberGuard.Web.Models;

public class SecurityActivity
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string ActivityType { get; set; } = "";
    public string Description { get; set; } = "";
    public string RelatedId { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
