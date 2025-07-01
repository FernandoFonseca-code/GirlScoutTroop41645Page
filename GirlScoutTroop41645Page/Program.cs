using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GirlScoutTroop41645Page.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using GirlScoutTroop41645Page.Services;
using GirlScoutTroop41645Page.Models;
using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.SignalR;


var builder = WebApplication.CreateBuilder(args);
var secrets = builder.Configuration.GetSection("GoogleCalendar").Get<Dictionary<string, string>>();

// Add services to the container.
string connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(o => 
{
    // This forces challenge results to be handled by Google OpenID Handler, so there's no
    // need to add an AccountController that emits challenges for Login.
    o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    // This forces forbid results to be handled by Google OpenID Handler, which checks if
    // extra scopes are required and does automatic incremental auth.
    o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    // Default scheme that will handle everything else.
    // Once a user is authenticated, the OAuth2 token info is stored in cookies.
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})

        .AddCookie()
        .AddGoogleOpenIdConnect(options =>
        {
            options.ClientId = secrets["ClientId"];
            options.ClientSecret = secrets["ClientSecret"];
            options.CallbackPath = "/signin-google";
        });

// Adds Google Calendar services
builder.Services.AddSingleton<GoogleCalendarService>();
// Adds email sender service
builder.Services.AddTransient<IEmailSender, EmailSender>();
// Adds Identity services
builder.Services.AddDefaultIdentity<Member>(options =>
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
builder.Services.AddSignalR().AddAzureSignalR(builder.Configuration["Azure:SignalR:ConnectionString"]!);
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
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

var serviceProvider = app.Services.GetRequiredService<IServiceProvider>().CreateScope();
//await Task.Run(async () =>
//{
//    await IdentityHelper.CreateRoles(serviceProvider.ServiceProvider, IdentityHelper.Admin, IdentityHelper.TroopLeader, IdentityHelper.TroopSectionLeader, IdentityHelper.Parent);
//    await IdentityHelper.CreateDefaultUser(serviceProvider.ServiceProvider, IdentityHelper.TroopLeader);
//});

app.Run();
