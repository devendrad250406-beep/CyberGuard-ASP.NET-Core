using System.Text.Json;
using CyberGuard.Web.Models;

namespace CyberGuard.Web.Services;

/// <summary>
/// Shared application data store. JSON persistence keeps this demo package-free.
/// For production deployments, replace this service with a database-backed repository.
/// </summary>
public class SecurityDataService
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public List<SecurityAlert> Alerts { get; private set; } = [];
    public List<SecurityIncident> Incidents { get; private set; } = [];
    public List<NetworkConnection> Connections { get; private set; } = [];
    public List<SecurityActivity> Activities { get; private set; } = [];

    public SecurityDataService(IWebHostEnvironment environment)
    {
        var dataDirectory = Path.Combine(environment.ContentRootPath, "Data");
        Directory.CreateDirectory(dataDirectory);
        _filePath = Path.Combine(dataDirectory, "security-data.json");
        Load();
        if (Alerts.Count == 0 && Incidents.Count == 0 && Connections.Count == 0)
        {
            Seed();
            Save();
        }
    }

    private sealed class Store
    {
        public List<SecurityAlert> Alerts { get; set; } = [];
        public List<SecurityIncident> Incidents { get; set; } = [];
        public List<NetworkConnection> Connections { get; set; } = [];
        public List<SecurityActivity> Activities { get; set; } = [];
    }

    private void Load()
    {
        if (!File.Exists(_filePath)) return;
        try
        {
            var store = JsonSerializer.Deserialize<Store>(File.ReadAllText(_filePath));
            if (store is null) return;
            Alerts = store.Alerts ?? [];
            Incidents = store.Incidents ?? [];
            Connections = store.Connections ?? [];
            Activities = store.Activities ?? [];
        }
        catch (JsonException)
        {
            // Keep the app available if a demo data file is malformed.
            Seed();
        }
    }

    private void Save()
    {
        var store = new Store { Alerts = Alerts, Incidents = Incidents, Connections = Connections, Activities = Activities };
        var json = JsonSerializer.Serialize(store, _jsonOptions);
        File.WriteAllText(_filePath, json);
    }

    private void Seed()
    {
        Alerts =
        [
            new() { Id="ALT-1001", ThreatType="Brute Force Attack", SourceIp="192.168.1.14", Severity="Critical", Status="Open", Description="Repeated failed login attempts detected." },
            new() { Id="ALT-1002", ThreatType="Malware Detection", SourceIp="10.0.0.23", Severity="High", Status="Open", Description="Suspicious executable behavior detected." },
            new() { Id="ALT-1003", ThreatType="Suspicious Login", SourceIp="172.16.0.8", Severity="Medium", Status="Investigating", Description="Login from an unusual location." },
            new() { Id="ALT-1004", ThreatType="Port Scanning", SourceIp="203.0.113.42", Severity="High", Status="Open", Description="Multiple ports probed in a short interval." },
            new() { Id="ALT-1005", ThreatType="SQL Injection", SourceIp="198.51.100.17", Severity="Critical", Status="Resolved", Description="SQL injection pattern found in request." },
            new() { Id="ALT-1006", ThreatType="Phishing Attempt", SourceIp="192.0.2.55", Severity="Low", Status="Resolved", Description="Reported phishing link." }
        ];
        Incidents =
        [
            new() { Id="INC-2001", Title="Repeated authentication failures", RelatedAlertId="ALT-1001", Priority="Critical", Status="Open", AssignedTo="Security Team", Notes="Review account lockout policy." },
            new() { Id="INC-2002", Title="Potential malware on endpoint", RelatedAlertId="ALT-1002", Priority="High", Status="Investigating", AssignedTo="Analyst 01", Notes="Endpoint isolation recommended." }
        ];
        Connections =
        [
            new() { Id="NET-3001", SourceIp="192.168.1.14", DestinationIp="10.0.0.5", Port=443, Protocol="TCP", Status="Active" },
            new() { Id="NET-3002", SourceIp="203.0.113.42", DestinationIp="10.0.0.10", Port=22, Protocol="TCP", Status="Active", IsSuspicious=true },
            new() { Id="NET-3003", SourceIp="10.0.0.23", DestinationIp="10.0.0.20", Port=53, Protocol="UDP", Status="Closed" },
            new() { Id="NET-3004", SourceIp="172.16.0.8", DestinationIp="10.0.0.5", Port=443, Protocol="TCP", Status="Active" }
        ];
        Activities =
        [
            new() { ActivityType="System Started", Description="CyberGuard monitoring workspace initialized.", RelatedId="SYS-001" },
            new() { ActivityType="New Alert", Description="Brute Force Attack alert created.", RelatedId="ALT-1001" },
            new() { ActivityType="Incident Created", Description="Repeated authentication failures assigned to Security Team.", RelatedId="INC-2001" }
        ];
    }

    private void AddActivity(string type, string description, string relatedId)
    {
        Activities.Insert(0, new SecurityActivity { ActivityType = type, Description = description, RelatedId = relatedId });
        if (Activities.Count > 200) Activities.RemoveAt(Activities.Count - 1);
    }

    public async Task AddAlertAsync(SecurityAlert alert)
    {
        await _gate.WaitAsync();
        try { alert.Id = $"ALT-{1000 + Alerts.Count + 1}"; Alerts.Insert(0, alert); AddActivity("New Alert", $"{alert.ThreatType} alert created.", alert.Id); Save(); }
        finally { _gate.Release(); }
    }

    public async Task UpdateAlertStatusAsync(string id, string status)
    {
        await _gate.WaitAsync();
        try
        {
            var item = Alerts.FirstOrDefault(x => x.Id == id);
            if (item is null) return;
            item.Status = status;
            AddActivity("Alert Status Updated", $"{id} status changed to {status}.", id);
            Save();
        }
        finally { _gate.Release(); }
    }

    public async Task AddIncidentAsync(SecurityIncident incident)
    {
        await _gate.WaitAsync();
        try { incident.Id = $"INC-{2000 + Incidents.Count + 1}"; Incidents.Insert(0, incident); AddActivity("Incident Created", $"{incident.Title} created.", incident.Id); Save(); }
        finally { _gate.Release(); }
    }

    public async Task UpdateIncidentAsync(string id, string status, string notes)
    {
        await _gate.WaitAsync();
        try
        {
            var item = Incidents.FirstOrDefault(x => x.Id == id);
            if (item is null) return;
            item.Status = status;
            if (!string.IsNullOrWhiteSpace(notes)) item.Notes = notes;
            AddActivity("Incident Updated", $"{id} status changed to {status}.", id);
            Save();
        }
        finally { _gate.Release(); }
    }

    public async Task AddConnectionAsync(NetworkConnection connection)
    {
        await _gate.WaitAsync();
        try { connection.Id = $"NET-{3000 + Connections.Count + 1}"; Connections.Insert(0, connection); AddActivity("Network Connection", $"{connection.SourceIp} connected to {connection.DestinationIp}:{connection.Port}.", connection.Id); Save(); }
        finally { _gate.Release(); }
    }

    public async Task MarkSuspiciousAsync(string id)
    {
        await _gate.WaitAsync();
        try
        {
            var item = Connections.FirstOrDefault(x => x.Id == id);
            if (item is null) return;
            item.IsSuspicious = true;
            AddActivity("Suspicious Connection", $"{id} marked as suspicious.", id);
            Save();
        }
        finally { _gate.Release(); }
    }

    public async Task GenerateTestAlertAsync()
    {
        var types = new[] { "Brute Force Attack", "Malware Detection", "Suspicious Login", "Port Scanning", "Phishing Attempt" };
        var random = Random.Shared;
        await AddAlertAsync(new SecurityAlert
        {
            ThreatType = types[random.Next(types.Length)],
            SourceIp = $"192.168.1.{random.Next(10, 240)}",
            Severity = new[] { "Low", "Medium", "High", "Critical" }[random.Next(4)],
            Status = "Open",
            Description = "Generated demonstration alert for testing."
        });
    }
}
