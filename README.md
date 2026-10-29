# CyberGuard — ASP.NET Core MVC

A web-based adaptation of the CyberGuard cybersecurity incident monitoring system. It contains six connected modules and uses a shared application data service so that activity in one module appears in the others.

## Modules

1. Overview Dashboard — alert, incident, and suspicious-connection summaries.
2. Threat Monitoring — create alerts, review severity, and update alert status.
3. Incident Management — create incidents, assign an owner, and update investigation status/notes.
4. Network Activity — add and flag simulated connection records.
5. Security Logs — centralized activity timeline across modules.
6. Analytics & Reports — summary metrics and CSV export.

## Technology

- ASP.NET Core MVC / C#
- Razor Views
- HTML5, CSS, JavaScript
- Built-in `System.Text.Json` persistence
- Dependency Injection
- Model-View-Controller architecture
- Anti-forgery validation on POST forms

No third-party NuGet packages are required. The project targets **.NET 10**.

## Run locally

1. Install the .NET 10 SDK.
2. Extract this repository.
3. Open a terminal in the folder containing `CyberGuard.Web.csproj`.
4. Run:

```bash
dotnet restore
dotnet run
```

5. Open the localhost URL printed in the terminal.

If your installed SDK is .NET 8, change `net10.0` to `net8.0` in `CyberGuard.Web.csproj`.

## Data storage

On first run, sample alerts, incidents, connections, and activity entries are created. The application stores subsequent changes in `Data/security-data.json` under the project folder. This file is intentionally excluded from Git by `.gitignore`, so each deployment starts with sample data unless you choose to version your demo data.

## Cross-module data synchronization

All controllers use the same singleton `SecurityDataService`. When an alert, incident, or network connection is created or updated, the service updates its shared collections and appends a security activity record. Dashboard, Security Logs, and Analytics read from that same data source.

## Important project scope

- Network Activity uses simulated connection records. It does **not** capture live packets or inspect real network traffic.
- JSON file persistence is intended for a student/demo project and a single application instance. For production or multi-user deployments, use a database such as SQL Server with Entity Framework Core, authentication/authorization, role-based access control, audit hardening, and secure hosting.
- The demo does not include user sign-in.

## GitHub upload

Create a new GitHub repository, extract this ZIP, then upload the contents of the `CyberGuard_ASPNETCore` folder (the `.csproj` should be at the repository root). Alternatively, use Git:

```bash
git init
git add .
git commit -m "Initial CyberGuard ASP.NET Core MVC project"
git branch -M main
git remote add origin https://github.com/YOUR-USERNAME/YOUR-REPOSITORY.git
git push -u origin main
```
