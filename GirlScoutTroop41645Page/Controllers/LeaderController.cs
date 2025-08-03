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
                TempData["Success"] = $"Successfully removed {role} role from {member.FirstName} {member.LastName}.";
            }
            else
            {
                TempData["Error"] = $"Failed to remove role: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}