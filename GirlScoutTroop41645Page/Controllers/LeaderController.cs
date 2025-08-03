using GirlScoutTroop41645Page.Data;
using GirlScoutTroop41645Page.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GirlScoutTroop41645Page.Controllers
{
    [Authorize(Roles = "TroopLeader")]
    public class LeaderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Member> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LeaderController(ApplicationDbContext context, UserManager<Member> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Leader
        public async Task<IActionResult> Index()
        {
            var members = await _context.Members.ToListAsync();
            var memberRoles = new Dictionary<string, IList<string>>();

            foreach (var member in members)
            {
                memberRoles[member.Id] = await _userManager.GetRolesAsync(member);
            }

            ViewBag.MemberRoles = memberRoles;
            ViewBag.AvailableRoles = new SelectList(new[]
            {
                new { Value = IdentityHelper.TroopLeader, Text = "Troop Leader" },
                new { Value = IdentityHelper.TroopSectionLeader, Text = "Troop Section Leader" },
                new { Value = IdentityHelper.Parent, Text = "Parent" }
            }, "Value", "Text");
            
            // Create TroopLevels list explicitly to prevent null reference
            ViewBag.TroopLevels = new List<object>
            {
                new { Value = "Daisy", Text = "Daisy" },
                new { Value = "Brownie", Text = "Brownie" },
                new { Value = "Junior", Text = "Junior" },
                new { Value = "Cadette", Text = "Cadette" },
                new { Value = "Senior", Text = "Senior" },
                new { Value = "Ambassador", Text = "Ambassador" }
            };

            return View(members);
        }

        // POST: Leader/AssignRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string memberId, string role)
        {
            if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(role))
            {
                TempData["Error"] = "Invalid member or role selection.";
                return RedirectToAction(nameof(Index));
            }

            var member = await _userManager.FindByIdAsync(memberId);
            if (member == null)
            {
                TempData["Error"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }

            // Check if role exists
            if (!await _roleManager.RoleExistsAsync(role))
            {
                TempData["Error"] = "Invalid role.";
                return RedirectToAction(nameof(Index));
            }

            // Check if user already has this role
            if (await _userManager.IsInRoleAsync(member, role))
            {
                TempData["Warning"] = $"Member {member.FirstName} {member.LastName} already has the {role} role.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.AddToRoleAsync(member, role);
            if (result.Succeeded)
            {
                TempData["Success"] = $"Successfully assigned {role} role to {member.FirstName} {member.LastName}.";
            }
            else
            {
                TempData["Error"] = $"Failed to assign role: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Leader/RemoveRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(string memberId, string role)
        {
            if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(role))
            {
                TempData["Error"] = "Invalid member or role selection.";
                return RedirectToAction(nameof(Index));
            }

            var member = await _userManager.FindByIdAsync(memberId);
            if (member == null)
            {
                TempData["Error"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }

            // Prevent removing TroopLeader role from the current user
            var currentUser = await _userManager.GetUserAsync(User);
            if (member.Id == currentUser?.Id && role == IdentityHelper.TroopLeader)
            {
                TempData["Error"] = "You cannot remove the TroopLeader role from yourself.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.RemoveFromRoleAsync(member, role);
            if (result.Succeeded)
            {
                // If removing TroopSectionLeader role, also clear subcategories
                if (role == IdentityHelper.TroopSectionLeader)
                {
                    member.TroopLevelSubcategories = null;
                    await _userManager.UpdateAsync(member);
                }
                
                TempData["Success"] = $"Successfully removed {role} role from {member.FirstName} {member.LastName}.";
            }
            else
            {
                TempData["Error"] = $"Failed to remove role: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToAction(nameof(Index));
        }
        
        // POST: Leader/AssignSubcategories
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignSubcategories(string memberId, List<string> subcategories)
        {
            if (string.IsNullOrEmpty(memberId))
            {
                TempData["Error"] = "Invalid member selection.";
                return RedirectToAction(nameof(Index));
            }

            var member = await _userManager.FindByIdAsync(memberId);
            if (member == null)
            {
                TempData["Error"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }

            // Check if member has TroopSectionLeader role
            if (!await _userManager.IsInRoleAsync(member, IdentityHelper.TroopSectionLeader))
            {
                TempData["Error"] = "Member must have TroopSectionLeader role to assign subcategories.";
                return RedirectToAction(nameof(Index));
            }

            // Validate subcategories (max 2)
            if (subcategories != null && subcategories.Count > 2)
            {
                TempData["Error"] = "A member can have maximum 2 troop level subcategories.";
                return RedirectToAction(nameof(Index));
            }

            // Validate that all subcategories are valid troop levels
            var validLevels = Enum.GetNames<TroopLevel>();
            var invalidSubcategories = subcategories?.Where(s => !validLevels.Contains(s)).ToList();
            if (invalidSubcategories?.Any() == true)
            {
                TempData["Error"] = $"Invalid subcategories: {string.Join(", ", invalidSubcategories)}";
                return RedirectToAction(nameof(Index));
            }

            // Update member's subcategories
            member.TroopLevelSubcategories = subcategories?.Any() == true ? string.Join(",", subcategories) : null;
            
            var result = await _userManager.UpdateAsync(member);
            if (result.Succeeded)
            {
                if (subcategories?.Any() == true)
                {
                    TempData["Success"] = $"Successfully assigned subcategories ({string.Join(", ", subcategories)}) to {member.FirstName} {member.LastName}.";
                }
                else
                {
                    TempData["Success"] = $"Successfully cleared subcategories for {member.FirstName} {member.LastName}.";
                }
            }
            else
            {
                TempData["Error"] = $"Failed to update subcategories: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}