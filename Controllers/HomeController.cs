using System.Diagnostics;
using CyberGuard.Web.Models;
using CyberGuard.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace CyberGuard.Web.Controllers;

public class HomeController(SecurityDataService data) : Controller
{
    public IActionResult Index(string section = "overview")
    {
        ViewBag.Section = section;
        return View(new DashboardViewModel
        {
            Alerts = data.Alerts.OrderByDescending(x => x.CreatedAt).ToList(),
            Incidents = data.Incidents.OrderByDescending(x => x.CreatedAt).ToList(),
            Connections = data.Connections.OrderByDescending(x => x.Timestamp).ToList(),
            Activities = data.Activities.OrderByDescending(x => x.Timestamp).ToList()
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAlert(string threatType, string sourceIp, string severity, string description)
    {
        await data.AddAlertAsync(new SecurityAlert { ThreatType=threatType, SourceIp=sourceIp, Severity=severity, Description=description });
        return RedirectToAction(nameof(Index), new { section="alerts" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAlert(string id, string status)
    {
        await data.UpdateAlertStatusAsync(id, status);
        return RedirectToAction(nameof(Index), new { section="alerts" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateIncident(string title, string relatedAlertId, string priority, string assignedTo, string notes)
    {
        await data.AddIncidentAsync(new SecurityIncident { Title=title, RelatedAlertId=relatedAlertId, Priority=priority, AssignedTo=assignedTo, Notes=notes });
        return RedirectToAction(nameof(Index), new { section="incidents" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateIncident(string id, string status, string notes)
    {
        await data.UpdateIncidentAsync(id, status, notes);
        return RedirectToAction(nameof(Index), new { section="incidents" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateConnection(string sourceIp, string destinationIp, int port, string protocol)
    {
        await data.AddConnectionAsync(new NetworkConnection { SourceIp=sourceIp, DestinationIp=destinationIp, Port=port, Protocol=protocol });
        return RedirectToAction(nameof(Index), new { section="network" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkSuspicious(string id)
    {
        await data.MarkSuspiciousAsync(id);
        return RedirectToAction(nameof(Index), new { section="network" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateTestAlert()
    {
        await data.GenerateTestAlertAsync();
        return RedirectToAction(nameof(Index), new { section="alerts" });
    }

    public IActionResult ExportCsv()
    {
        var rows = new List<string> { "Type,ID,Description,Status,Timestamp" };
        rows.AddRange(data.Alerts.Select(a => $"Alert,{Csv(a.Id)},{Csv(a.ThreatType + " - " + a.SourceIp)},{Csv(a.Status)},{a.CreatedAt:O}"));
        rows.AddRange(data.Incidents.Select(i => $"Incident,{Csv(i.Id)},{Csv(i.Title)},{Csv(i.Status)},{i.CreatedAt:O}"));
        rows.AddRange(data.Activities.Select(a => $"Activity,{Csv(a.RelatedId)},{Csv(a.Description)},{Csv(a.ActivityType)},{a.Timestamp:O}"));
        return File(System.Text.Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, rows)), "text/csv", $"CyberGuard-Report-{DateTime.Now:yyyyMMdd-HHmm}.csv");
    }

    private static string Csv(string value) => "\"" + (value ?? "").Replace("\"", "\"\"") + "\"";

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
