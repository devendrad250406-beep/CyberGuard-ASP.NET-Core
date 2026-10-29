namespace CyberGuard.Web.Models;

public class NetworkConnection
{
    public string Id { get; set; } = "";
    public string SourceIp { get; set; } = "";
    public string DestinationIp { get; set; } = "";
    public int Port { get; set; }
    public string Protocol { get; set; } = "TCP";
    public string Status { get; set; } = "Active";
    public bool IsSuspicious { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
