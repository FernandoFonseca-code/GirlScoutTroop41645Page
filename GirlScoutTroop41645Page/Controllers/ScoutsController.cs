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

namespace GirlScoutTroop41645Page.Controllers
{
    [Authorize]
    public class ScoutsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScoutsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Scouts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Scouts.ToListAsync());
        }

        // GET: Scouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scout = await _context.Scouts
                .FirstOrDefaultAsync(m => m.ScoutId == id);
            if (scout == null)
            {
                return NotFound();
            }

            return View(scout);
        }

        // GET: Scouts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Scouts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScoutId,FirstName,LastName,DateOfBirth,Level")] Scout scout)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scout);
                await _context.SaveChangesAsync();
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

            var scout = await _context.Scouts.FindAsync(id);
            if (scout == null)
            {
                return NotFound();
            }
            return View(scout);
        }

        // POST: Scouts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScoutId,FirstName,LastName,DateOfBirth,Level")] Scout scout)
        {
            if (id != scout.ScoutId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
                .FirstOrDefaultAsync(m => m.ScoutId == id);
            if (scout == null)
            {
                return NotFound();
            }

            return View(scout);
        }

        // POST: Scouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
    }
}
