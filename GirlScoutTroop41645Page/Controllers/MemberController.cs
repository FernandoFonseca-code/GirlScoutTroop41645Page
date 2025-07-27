using GirlScoutTroop41645Page.Data;
using GirlScoutTroop41645Page.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GirlScoutTroop41645Page.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Member> _userManager;

        public MemberController(ApplicationDbContext context, UserManager<Member> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Member
        public async Task<IActionResult> Index()
        {
            var members = await _context.Members.ToListAsync();
            return View(members);
        }

        // GET: Member/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.Scouts)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Member/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            
            return View(member);
        }

        // POST: Member/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Member member)
        {
            if (id != member.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMember = await _context.Members.FindAsync(id);
                    
                    // Update only allowed fields
                    existingMember.FirstName = member.FirstName;
                    existingMember.LastName = member.LastName;
                    existingMember.Address = member.Address;
                    existingMember.City = member.City;
                    existingMember.State = member.State;
                    existingMember.ZipCode = member.ZipCode;
                    existingMember.PhoneNumber = member.PhoneNumber;
                    
                    _context.Update(existingMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.Id))
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
            return View(member);
        }

        // GET: Member/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Member/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(string id)
        {
            return _context.Members.Any(e => e.Id == id);
        }
    }
}