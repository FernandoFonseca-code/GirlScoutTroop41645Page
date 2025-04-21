using Microsoft.AspNetCore.Identity;

namespace GirlScoutTroop41645Page.Models;

public static class IdentityHelper
{
    public const string TroopLeader = "TroopLeader";
    public const string Parent = "Parent";

    public static async Task CreateRoles(IServiceProvider provider, params string[] roles)
    {
       RoleManager<IdentityRole> roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (string role in roles)
        {
            bool doesRoleExist = await roleManager.RoleExistsAsync(role);
            if (!doesRoleExist)
            {
                IdentityRole identityRole = new IdentityRole(role);
                IdentityResult result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    Console.WriteLine($"Role {role} created successfully.");
                }
                else
                {
                    Console.WriteLine($"Error creating role {role}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                Console.WriteLine($"Role {role} already exists.");
            }
        }
    }
}
