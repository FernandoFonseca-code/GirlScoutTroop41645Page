using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GirlScoutTroop41645Page.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using GirlScoutTroop41645Page.Services;
using GirlScoutTroop41645Page.Models;


var builder = WebApplication.CreateBuilder(args);
/// <summary>
/// Lines 14-22 are used because I am using a custom path for the appsettings.json file.
/// Had to update the path and create a new configuration builder to use the new path as IConfiguration
/// could not be used anymore.
/// </summary>
// Set up configuration to use the specific appsettings.json file path
string appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "AppData", "appsettings.json");

// Create a new ConfigurationBuilder and add the file
var configurationBuilder = new ConfigurationBuilder()
    .AddJsonFile(appSettingsPath, optional: false, reloadOnChange: true);

// Build the configuration and add it to the builder
var configuration = configurationBuilder.Build();
builder.Configuration.AddConfiguration(configuration);

// Add services to the container.
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpContextAccessor();

// Adds Google Calendar services
builder.Services.AddSingleton<GoogleCalendarService>();
// Adds email sender service
builder.Services.AddTransient<IEmailSender, EmailSender>();
// Adds Identity services
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{ 
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
})
.AddRoles<IdentityRole>() // This line adds role management support
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    // Parent settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._!@+";
    options.User.RequireUniqueEmail = false;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

var serviceProvider = app.Services.GetRequiredService<IServiceProvider>().CreateScope();
// Create roles
await IdentityHelper.CreateRoles(serviceProvider.ServiceProvider, IdentityHelper.TroopLeader, IdentityHelper.TroopSectionLeader, IdentityHelper.Parent);

//Create default Troop Leader
await IdentityHelper.CreateDefaultUser(serviceProvider.ServiceProvider, IdentityHelper.TroopLeader);

app.Run();
