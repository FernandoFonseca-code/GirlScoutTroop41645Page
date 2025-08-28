# Girl Scout Troop 41645 Website

An ASP.NET Core 8.0 web application for Girl Scout Troop 41645 that provides calendar management, picture repository, membership dues processing, form submissions, and overall community involvement features. The application uses Entity Framework Core with SQL Server, ASP.NET Core Identity with Google OAuth authentication, Azure SignalR, Google Calendar API integration, and SendGrid for email services.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites and Installation
- .NET 8.0 SDK is required and available on this system
- Entity Framework Core tools are required: `dotnet tool install --global dotnet-ef`

### Bootstrap, Build, and Test the Repository
- `dotnet restore` -- takes ~30 seconds for first restore, ~1 second for subsequent runs. NEVER CANCEL.
- `dotnet build` -- takes ~4-5 seconds and succeeds with 31 nullable reference warnings (these are expected). NEVER CANCEL.
- For clean build: `dotnet clean && dotnet build` -- takes ~5 seconds total
- No unit test projects exist in this repository
- No GitHub Actions workflows exist in this repository

### Database Configuration
- **CRITICAL LIMITATION**: The application is configured to use SQL Server LocalDB which is NOT supported on Linux
- Default connection string: `Server=(localdb)\\mssqllocaldb;Database=GSTroop41645Db;Trusted_Connection=True;MultipleActiveResultSets=true`
- The application CANNOT run on Linux without database configuration changes
- On Windows: Use `dotnet-ef database update` to create the database
- On Linux: Database operations will fail - document this limitation rather than attempting workarounds

### Running the Application
- **WINDOWS ONLY**: `dotnet run` after successful database setup
- **LINUX**: Application will crash during startup due to LocalDB dependency
- Default ports: HTTP 5099, HTTPS 7071
- Environment variables required for full functionality (see Configuration section)

### Configuration Requirements
The application requires several external service configurations that are stored in user secrets:
- **Google Calendar API**: ClientId, ClientSecret, CalendarId, ApiKey, ApplicationName, etc.
- **SendGrid Email**: ApiKey, senderEmail
- **Azure SignalR**: ConnectionString
- Without these configurations, the application may start but many features will be non-functional

## Validation
- Build validation: Check that `dotnet build` completes successfully with expected 31 warnings
- **CANNOT validate runtime functionality on Linux** due to LocalDB dependency
- On Windows: Navigate to https://localhost:7071 or http://localhost:5099 to verify the application loads
- The application includes role-based authentication with three roles: Troop Leader, Troop Section Leader, and Parent
- Always run `dotnet build` before committing changes to ensure no new compilation errors

### Manual Testing Scenarios (Windows Only)
After making changes, test these key user workflows:
- **Anonymous User**: Visit home page, view public information, attempt to access member areas (should redirect to login)
- **Parent Role**: Login via Google OAuth, access member dashboard, view scout information
- **Troop Leader Role**: Access administrative functions, manage scouts, calendar events, and member communications
- **Calendar Integration**: Verify Google Calendar events display correctly on the calendar page
- **Email Features**: Test email sending functionality (requires SendGrid configuration)

## Key Components

### Project Structure
```
GirlScoutTroop41645Page/
├── Controllers/           # MVC Controllers (Calendar, Email, Home, Leader, Member, Scouts)
├── Models/               # Data models (Member, Scout, Identity helpers)
├── Views/                # Razor views and layouts
├── Services/             # Business logic (EmailSender, GoogleCalendarService)
├── Data/                 # ApplicationDbContext
├── Migrations/           # Entity Framework migrations
├── Areas/Identity/       # ASP.NET Core Identity UI
├── wwwroot/             # Static web assets
└── Program.cs           # Application startup and configuration
```

### Main Controllers
- **HomeController**: Landing page and general navigation
- **CalendarController**: Google Calendar integration
- **MemberController**: Member management functionality  
- **ScoutsController**: Scout profile and troop management
- **LeaderController**: Troop leader administration features
- **EmailController**: Email communication features

### Key Models
- **Member**: ASP.NET Core Identity user with additional troop member properties
- **Scout**: Scout profile information linked to members
- **IdentityHelper**: Role management and default user creation

## Common Development Tasks

### Making Code Changes
- Always run `dotnet build` after making changes to verify no compilation errors
- The application has nullable reference types enabled, expect warnings for null reference scenarios
- Authentication and authorization changes should consider the three role hierarchy
- Database changes require new Entity Framework migrations

### Available Commands
- `dotnet sln list` -- shows the single project in the solution
- `dotnet build GirlScoutTroop41645Page.sln` -- builds from solution file
- `dotnet run --project GirlScoutTroop41645Page/GirlScoutTroop41645Page.csproj` -- runs specific project
- No linting or formatting tools are configured in the project

### Database Migrations
- **Windows only**: `dotnet-ef migrations add <MigrationName>` to create new migrations
- **Windows only**: `dotnet-ef database update` to apply migrations
- **Linux**: Migration commands will fail due to LocalDB dependency
- Current migrations create ASP.NET Identity tables and Scout/Member relationship tables

### Troubleshooting Build Issues
- Restore packages if build fails: `dotnet restore` (takes ~30 seconds for first run)
- Clean build if needed: `dotnet clean && dotnet build` (takes ~5 seconds total)
- Check for missing NuGet package references in .csproj file
- Common warnings about nullable references are expected and do not prevent compilation
- Build warnings count should remain at 31 - investigate if this number changes significantly

## Environment-Specific Notes

### Windows Development
- Full application functionality available
- LocalDB works for development database
- Entity Framework commands functional
- Can run and test complete application features

### Linux Development  
- **LIMITATION**: Cannot run the application due to LocalDB dependency
- Can perform code editing, building, and static analysis
- Cannot test runtime functionality or database operations
- Focus development on code structure, logic, and compilation correctness

## Configuration Files Reference

### Key Files
- `appsettings.json`: Main application configuration with connection strings and service settings
- `appsettings.Development.json`: Development-specific overrides
- `GirlScoutTroop41645Page.csproj`: Project file with .NET 8.0 target and package references
- `Program.cs`: Application startup, service registration, and authentication configuration
- `Properties/launchSettings.json`: Development server configuration (ports 5099 HTTP, 7071 HTTPS)

### Repository Statistics
- 61 C# source files
- 60 Razor view files (.cshtml)
- Single project solution
- No test projects
- No CI/CD pipelines configured

### Package Dependencies
- Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.11
- Microsoft.EntityFrameworkCore.SqlServer 8.0.11
- Google.Apis.Calendar.v3 1.69.0.3746
- Google.Apis.Auth.AspNetCore3 1.69.0
- Microsoft.Azure.SignalR 1.30.3
- SendGrid 9.29.3

Always check these instructions first before running bash commands or making assumptions about the codebase functionality.