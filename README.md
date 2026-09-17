# GirlScoutTroop41645Page
This is my first project with an actual client. My daughter's Girl Scout (GS) Troop Leader asked me to create a website for the troop to assist with disseminating information.

## Description
This is a one-stop landing page for everything that Troop 41645 needs, including calendar management, picture repository, membership dues processing, form submissions, and overall community involvement. The goal is to create a standard for GS family communication and engagement.

## WEBSITE PREVIEW
![LandingPage](/GirlScoutPageLandingPage.jpg)
![LandingPage2](/GirlScoutPageLandingPage2.jpg)

## Azure deployment readiness notes

### What was adjusted in this repo for deployment safety
- Startup now fails early with clear errors when required production settings are missing (`ConnectionStrings:DatabaseConnection`, Google OAuth client settings, and Azure SignalR connection string when enabled).
- Google Calendar token storage no longer uses a hardcoded local Windows path and now supports configurable token storage through `GoogleCalendar:TokenPath`.
- Default Troop Leader account seeding is now opt-in via configuration (`SeedData:CreateDefaultTroopLeader`) to avoid creating privileged accounts automatically in production.

### Required Azure resources
- Azure App Service (Web App)
- Azure SQL Database
- Azure SignalR Service (if real-time SignalR is required)

### Required App Service configuration (set these yourself)
Set these values in **App Service > Configuration** (Application settings / Connection strings):

- `ConnectionStrings__DatabaseConnection` (Azure SQL connection string)
- `GoogleCalendar__ClientId`
- `GoogleCalendar__ClientSecret`
- `GoogleCalendar__CalendarId`
- `GoogleCalendar__ApplicationName`
- `GoogleCalendar__RedirectUri` (must match your deployed site auth callback expectations)
- `GoogleCalendar__TokenPath` (recommended persistent Linux path: `/home/data/google-calendar-tokens`)
- `SendGrid_Sender__ApiKey`
- `SendGrid_Sender__senderEmail`
- `Azure__SignalR__Enabled` (`true` or `false`)
- `Azure__SignalR__ConnectionString` (required only when SignalR is enabled)
- `SeedData__CreateDefaultTroopLeader` (`false` for normal production usage)
- `SeedData__TroopLeaderPassword` (required only if seeding is intentionally enabled)

### Authentication and external provider setup you must complete
- In Google Cloud Console, configure OAuth consent and credentials.
- Add the deployed callback URI for this app (`https://<your-domain>/signin-google`) to allowed redirect URIs.
- Ensure Google Calendar API is enabled on the same Google project.
- Ensure SendGrid sender identity/domain is verified for your sender address.

### Database deployment
- Apply EF Core migrations against Azure SQL before first production use.
- Recommended from a trusted environment:
  - `dotnet tool install --global dotnet-ef` (if needed)
  - `dotnet ef database update --project /home/runner/work/GirlScoutTroop41645Page/GirlScoutTroop41645Page/GirlScoutTroop41645Page/GirlScoutTroop41645Page.csproj`

### Optional hardening recommendations
- Keep `SeedData__CreateDefaultTroopLeader=false` after initial setup.
- Store sensitive settings in Azure Key Vault and reference them from App Service.
- Enable App Service diagnostic logs and failed request tracing.
- Add health checks and monitoring (Application Insights) for production observability.
