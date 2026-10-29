namespace CyberGuard.Web.Models;

public class DashboardViewModel
{
    public List<SecurityAlert> Alerts { get; set; } = [];
    public List<SecurityIncident> Incidents { get; set; } = [];
    public List<NetworkConnection> Connections { get; set; } = [];
    public List<SecurityActivity> Activities { get; set; } = [];
    public int CriticalAlerts => Alerts.Count(a => a.Severity == "Critical");
    public int OpenIncidents => Incidents.Count(i => i.Status is "Open" or "Investigating");
    public int ResolvedAlerts => Alerts.Count(a => a.Status == "Resolved");
    public int SuspiciousConnections => Connections.Count(c => c.IsSuspicious);
}
