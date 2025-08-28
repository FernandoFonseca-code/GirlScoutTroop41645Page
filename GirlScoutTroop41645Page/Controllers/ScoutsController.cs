using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GirlScoutTroop41645Page.Data;
using GirlScoutTroop41645Page.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GirlScoutTroop41645Page.Controllers
{
    [Authorize]
    public class ScoutsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Member> _userManager;

        public ScoutsController(ApplicationDbContext context, UserManager<Member> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Scouts
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var scouts = new List<Scout>();

            if (User.IsInRole(IdentityHelper.TroopLeader))
            {
                // TroopLeader can see all scouts
                scouts = await _context.Scouts.Include(s => s.Members).ToListAsync();
            }
            else if (User.IsInRole(IdentityHelper.TroopSectionLeader))
            {
                // TroopSectionLeader can see scouts based on their level
                // For now, showing all scouts - could be enhanced to filter by specific levels
                scouts = await _context.Scouts.Include(s => s.Members).ToListAsync();
            }
            else if (User.IsInRole(IdentityHelper.Parent))
            {
                // Parents can only see scouts linked to them
                scouts = await _context.Scouts
                    .Include(s => s.Members)
                    .Where(s => s.Members.Any(m => m.Id == currentUser.Id))
                    .ToListAsync();
            }

            return View(scouts);
        }

        // GET: Scouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scout = await _context.Scouts
                .Include(s => s.Members)
                .FirstOrDefaultAsync(m => m.ScoutId == id);
            
            if (scout == null)
            {
                return NotFound();
            }

            // Check if user has permission to view this scout
            if (!await CanAccessScout(scout.ScoutId))
            {
                return Forbid();
            }

            return View(scout);
        }

        // GET: Scouts/Create
        public IActionResult Create()
        {
            // Only TroopLeader, TroopSectionLeader, and Parents can create scouts
            if (!User.IsInRole(IdentityHelper.TroopLeader) && 
                !User.IsInRole(IdentityHelper.TroopSectionLeader) && 
                !User.IsInRole(IdentityHelper.Parent))
            {
                return Forbid();
            }

            return View();
        }

        // POST: Scouts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScoutId,FirstName,LastName,DateOfBirth,Level")] Scout scout)
        {
            // Only TroopLeader, TroopSectionLeader, and Parents can create scouts
            if (!User.IsInRole(IdentityHelper.TroopLeader) && 
                !User.IsInRole(IdentityHelper.TroopSectionLeader) && 
                !User.IsInRole(IdentityHelper.Parent))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                _context.Add(scout);
                await _context.SaveChangesAsync();

                // If the user is a Parent, automatically link the scout to them
                if (User.IsInRole(IdentityHelper.Parent))
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser != null)
                    {
                        // Load the scout with members to avoid tracking issues
                        var savedScout = await _context.Scouts
                            .Include(s => s.Members)
                            .FirstOrDefaultAsync(s => s.ScoutId == scout.ScoutId);
                        
                        if (savedScout != null && !savedScout.Members.Any(m => m.Id == currentUser.Id))
                        {
                            savedScout.Members.Add(currentUser);
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(scout);
        }

        // GET: Scouts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scout = await _context.Scouts.AsNoTracking().FirstOrDefaultAsync(s => s.ScoutId == id);
            if (scout == null)
            {
                return NotFound();
            }

            // Check if user has permission to edit this scout
            if (!await CanAccessScout(scout.ScoutId))
            {
                return Forbid();
            }

            return View(scout);
        }

        // POST: Scouts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScoutId,FirstName,LastName,DateOfBirth,Level")] Scout scout)
        {
            if (id != scout.ScoutId)
            {
                return NotFound();
            }

            // Check if user has permission to edit this scout
            if (!await CanAccessScout(scout.ScoutId))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Detach any existing tracked entity with the same key
                    var existingEntity = _context.Entry(_context.Scouts.Local.FirstOrDefault(s => s.ScoutId == id));
                    if (existingEntity != null)
                    {
                        existingEntity.State = EntityState.Detached;
                    }

                    _context.Update(scout);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScoutExists(scout.ScoutId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(scout);
        }

        // GET: Scouts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scout = await _context.Scouts
                .Include(s => s.Members)
                .FirstOrDefaultAsync(m => m.ScoutId == id);
            
            if (scout == null)
            {
                return NotFound();
            }

            // Check if user has permission to delete this scout
            if (!await CanAccessScout(scout.ScoutId))
            {
                return Forbid();
            }

            return View(scout);
        }

        // POST: Scouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Check if user has permission to delete this scout
            if (!await CanAccessScout(id))
            {
                return Forbid();
            }

            var scout = await _context.Scouts.FindAsync(id);
            if (scout != null)
            {
                _context.Scouts.Remove(scout);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScoutExists(int id)
        {
            return _context.Scouts.Any(e => e.ScoutId == id);
        }

        // Helper method to check if current user can access a scout
        private async Task<bool> CanAccessScout(int scoutId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            // TroopLeader can access all scouts
            if (User.IsInRole(IdentityHelper.TroopLeader))
            {
                return true;
            }
            
            // TroopSectionLeader can access scouts (for now all, could be enhanced to filter by level)
            if (User.IsInRole(IdentityHelper.TroopSectionLeader))
            {
                return true;
            }
            
            // Parents can only access scouts linked to them
            if (User.IsInRole(IdentityHelper.Parent))
            {
                var scout = await _context.Scouts
                    .Include(s => s.Members)
                    .FirstOrDefaultAsync(s => s.ScoutId == scoutId);
                
                return scout?.Members.Any(m => m.Id == currentUser.Id) == true;
            }
            
            return false;
        }
    }
}
