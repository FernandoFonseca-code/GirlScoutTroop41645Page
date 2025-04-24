using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace GirlScoutTroop41645Page.Models;

public class IdentityHelper
{
    public const string TroopLeader = "TroopLeader";
    public const string TroopSectionLeader = "TroopSectionLeader";
    public const string Parent = "Parent";

    public static async Task CreateRoles(IServiceProvider provider, params string[] roles)
    {
        RoleManager<IdentityRole> roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (string role in roles)
        {
            bool doesRoleExist = await roleManager.RoleExistsAsync(role);
            if (!doesRoleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static async Task CreateDefaultUser(IServiceProvider provider, string role)
    {
        var UserManager = provider.GetService<UserManager<IdentityUser>>();
        var configuration = provider.GetService<IConfiguration>();

        string password = configuration["TroopLeaderPassword:password"];
        // checks to see how many users are in the specified role
        int numUsers = (await UserManager.GetUsersInRoleAsync(role)).Count();
        if (numUsers == 0)
        {
            var defaultUser = new IdentityUser()
            {
                Email = "gstroop41645@gmail.com",
                UserName = "TroopLeader",
                EmailConfirmed = true,
            };
            await UserManager.CreateAsync(defaultUser,password);

            await UserManager.AddToRoleAsync(defaultUser, role);
        }
    }
}
