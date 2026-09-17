# Copilot Cloud Agent Instructions for `GirlScoutTroop41645Page`

## Project at a glance
- ASP.NET Core MVC app targeting **.NET 8** (`GirlScoutTroop41645Page/GirlScoutTroop41645Page.csproj`).
- Single-solution repository: `GirlScoutTroop41645Page.sln`.
- Main app path: `/home/runner/work/GirlScoutTroop41645Page/GirlScoutTroop41645Page/GirlScoutTroop41645Page`.
- Key features: ASP.NET Core Identity, Google OAuth, Google Calendar integration, SendGrid email, Azure SignalR.

## Fastest reliable workflow for first-time agents
1. `dotnet restore`
2. `dotnet build`
3. Make changes.
4. `dotnet build` again before finalizing.

Notes:
- Build currently succeeds with **31 warnings, 0 errors** (warnings are pre-existing nullable/obsolete warnings).
- There are **no test projects** in this repository.
- There are **no GitHub Actions workflows** currently checked into `.github/workflows`.

## Commands verified in this repo
Run from repo root: `/home/runner/work/GirlScoutTroop41645Page/GirlScoutTroop41645Page`

- Restore:
  - `dotnet restore`
- Build:
  - `dotnet build`
  - `dotnet build GirlScoutTroop41645Page.sln`
- Run locally (Windows-only runtime path due to LocalDB):
  - `dotnet run --project GirlScoutTroop41645Page/GirlScoutTroop41645Page.csproj`

## Critical environment limitation (Linux cloud agents)
The app uses SQL Server LocalDB by default:
- `ConnectionStrings:DatabaseConnection` in `GirlScoutTroop41645Page/appsettings.json`
- Value: `Server=(localdb)\\mssqllocaldb;...`

LocalDB is not supported on Linux, so runtime and DB operations fail in cloud-agent Linux environments.

## Errors encountered during onboarding and work-around
### Encountered error
When running the app on Linux:
- Command: `dotnet run --project GirlScoutTroop41645Page/GirlScoutTroop41645Page.csproj`
- Failure: `System.PlatformNotSupportedException: LocalDB is not supported on this platform.`
- Startup fails while seeding identity roles/users in `Program.cs` via `IdentityHelper.CreateRoles(...)`.

### Work-around used
- Do not run runtime validation on Linux for this repository.
- Validate changes using `dotnet build` only.
- If runtime verification is required, run on Windows with LocalDB available, then apply migrations:
  - `dotnet tool install --global dotnet-ef`
  - `dotnet-ef database update`
  - `dotnet run`

## Secrets/configuration expected for full feature behavior
The app reads integration settings from configuration/user secrets:
- `GoogleCalendar:*` (ClientId, ClientSecret, CalendarId, etc.)
- `SendGrid_Sender:*` (ApiKey, senderEmail)
- `Azure:SignalR:ConnectionString`

Without these, the app may compile but integrated features are non-functional.

## High-value files for common tasks
- Startup/configuration: `GirlScoutTroop41645Page/Program.cs`
- EF Core context: `GirlScoutTroop41645Page/Data/ApplicationDbContext.cs`
- Identity role/user seeding: `GirlScoutTroop41645Page/Models/IdentityHelper.cs`
- MVC controllers: `GirlScoutTroop41645Page/Controllers/`
- Razor views: `GirlScoutTroop41645Page/Views/`
- EF migrations: `GirlScoutTroop41645Page/Migrations/`

## Change guidance
- Keep edits minimal and scoped.
- Do not introduce alternate local database workarounds unless explicitly requested.
- After edits, always run `dotnet build` to ensure no new compile errors.
