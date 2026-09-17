using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GirlScoutTroop41645Page.Data;
using GirlScoutTroop41645Page.Models;
using System.Text.Json;

namespace GirlScoutTroop41645Page.Controllers;

[Authorize]
public class FormsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Member> _userManager;

    public FormsController(ApplicationDbContext context, UserManager<Member> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Forms
    public async Task<IActionResult> Index()
    {
        var forms = await _context.Forms
            .Include(f => f.FormSubmissions)
            .OrderByDescending(f => f.CreatedDate)
            .ToListAsync();

        return View(forms);
    }

    // GET: Forms/Available (for members to fill out)
    public async Task<IActionResult> Available()
    {
        var userId = _userManager.GetUserId(User);
        var activeForms = await _context.Forms
            .Where(f => f.IsActive && (f.DueDate == null || f.DueDate > DateTime.UtcNow))
            .Include(f => f.FormSubmissions)
            .ToListAsync();

        // Check which forms the user has already submitted
        var submittedFormIds = await _context.FormSubmissions
            .Where(fs => fs.SubmittedBy == userId)
            .Select(fs => fs.FormId)
            .ToListAsync();

        ViewBag.SubmittedFormIds = submittedFormIds;
        return View(activeForms);
    }

    // GET: Forms/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var form = await _context.Forms
            .Include(f => f.FormFields.OrderBy(ff => ff.DisplayOrder))
            .Include(f => f.FormSubmissions)
                .ThenInclude(fs => fs.SubmittedByMember)
            .FirstOrDefaultAsync(m => m.FormId == id);

        if (form == null)
        {
            return NotFound();
        }

        return View(form);
    }

    // GET: Forms/Create
    [Authorize(Roles = "TroopLeader,TroopSectionLeader")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Forms/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TroopLeader,TroopSectionLeader")]
    public async Task<IActionResult> Create([Bind("Title,Description,DueDate,IsActive")] Form form)
    {
        if (ModelState.IsValid)
        {
            form.CreatedBy = _userManager.GetUserId(User)!;
            form.CreatedDate = DateTime.UtcNow;
            _context.Add(form);
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", new { id = form.FormId });
        }
        return View(form);
    }

    // GET: Forms/Edit/5
    [Authorize(Roles = "TroopLeader,TroopSectionLeader")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var form = await _context.Forms
            .Include(f => f.FormFields.OrderBy(ff => ff.DisplayOrder))
            .FirstOrDefaultAsync(f => f.FormId == id);

        if (form == null)
        {
            return NotFound();
        }

        return View(form);
    }

    // GET: Forms/Fill/5
    public async Task<IActionResult> Fill(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        
        // Check if user has already submitted this form
        var existingSubmission = await _context.FormSubmissions
            .AnyAsync(fs => fs.FormId == id && fs.SubmittedBy == userId);

        if (existingSubmission)
        {
            TempData["Message"] = "You have already submitted this form.";
            return RedirectToAction(nameof(Available));
        }

        var form = await _context.Forms
            .Include(f => f.FormFields.OrderBy(ff => ff.DisplayOrder))
            .FirstOrDefaultAsync(f => f.FormId == id && f.IsActive);

        if (form == null || (form.DueDate.HasValue && form.DueDate < DateTime.UtcNow))
        {
            return NotFound();
        }

        return View(form);
    }

    // POST: Forms/Submit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(int id, Dictionary<int, string> responses)
    {
        var userId = _userManager.GetUserId(User);
        
        // Check if user has already submitted this form
        var existingSubmission = await _context.FormSubmissions
            .AnyAsync(fs => fs.FormId == id && fs.SubmittedBy == userId);

        if (existingSubmission)
        {
            TempData["Message"] = "You have already submitted this form.";
            return RedirectToAction(nameof(Available));
        }

        var form = await _context.Forms
            .Include(f => f.FormFields)
            .FirstOrDefaultAsync(f => f.FormId == id && f.IsActive);

        if (form == null)
        {
            return NotFound();
        }

        // Validate required fields
        foreach (var field in form.FormFields.Where(f => f.IsRequired))
        {
            if (!responses.ContainsKey(field.FormFieldId) || 
                string.IsNullOrWhiteSpace(responses[field.FormFieldId]))
            {
                ModelState.AddModelError($"responses[{field.FormFieldId}]", $"{field.Label} is required.");
            }
        }

        if (!ModelState.IsValid)
        {
            return View("Fill", form);
        }

        // Create submission
        var submission = new FormSubmission
        {
            FormId = id,
            SubmittedBy = userId!,
            SubmittedDate = DateTime.UtcNow
        };

        _context.FormSubmissions.Add(submission);
        await _context.SaveChangesAsync();

        // Create field responses
        foreach (var response in responses)
        {
            if (!string.IsNullOrWhiteSpace(response.Value))
            {
                var fieldResponse = new FormFieldResponse
                {
                    FormSubmissionId = submission.FormSubmissionId,
                    FormFieldId = response.Key,
                    Response = response.Value
                };
                _context.FormFieldResponses.Add(fieldResponse);
            }
        }

        await _context.SaveChangesAsync();

        TempData["Message"] = "Form submitted successfully!";
        return RedirectToAction(nameof(Available));
    }

    // GET: Forms/Submissions/5
    [Authorize(Roles = "TroopLeader,TroopSectionLeader")]
    public async Task<IActionResult> Submissions(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var form = await _context.Forms
            .Include(f => f.FormFields.OrderBy(ff => ff.DisplayOrder))
            .Include(f => f.FormSubmissions)
                .ThenInclude(fs => fs.SubmittedByMember)
            .Include(f => f.FormSubmissions)
                .ThenInclude(fs => fs.FormFieldResponses)
                    .ThenInclude(ffr => ffr.FormField)
            .FirstOrDefaultAsync(f => f.FormId == id);

        if (form == null)
        {
            return NotFound();
        }

        return View(form);
    }

    // POST: Forms/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TroopLeader,TroopSectionLeader")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var form = await _context.Forms.FindAsync(id);
        if (form != null)
        {
            _context.Forms.Remove(form);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}