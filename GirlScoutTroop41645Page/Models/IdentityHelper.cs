using Microsoft.AspNetCore.Identity;

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
        var UserManager = provider.GetRequiredService<UserManager<Member>>();
        var configuration = provider.GetRequiredService<IConfiguration>();

        string password = configuration["GoogleCalendar:TroopLeaderPassword"];
        // checks to see how many users are in the specified role
        int numUsers = (await UserManager.GetUsersInRoleAsync(role)).Count();
        if (numUsers == 0)
        {
            var defaultUser = new Member()
            {
                Email = "gstroop41645@gmail.com",
                UserName = "TroopLeader",
                EmailConfirmed = true,
            };
            await UserManager.CreateAsync(defaultUser, password);
            await UserManager.AddToRoleAsync(defaultUser, role);
        }
    }
}
