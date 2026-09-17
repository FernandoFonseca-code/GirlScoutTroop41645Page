using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GirlScoutTroop41645Page.Data;
using GirlScoutTroop41645Page.Models;

namespace GirlScoutTroop41645Page.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "TroopLeader,TroopSectionLeader")]
public class FormFieldsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FormFieldsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/FormFields
    [HttpPost]
    public async Task<ActionResult<FormField>> PostFormField(FormField formField)
    {
        // Get the next display order for this form
        var maxOrder = await _context.FormFields
            .Where(ff => ff.FormId == formField.FormId)
            .Select(ff => ff.DisplayOrder)
            .DefaultIfEmpty(0)
            .MaxAsync();

        formField.DisplayOrder = maxOrder + 1;

        _context.FormFields.Add(formField);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetFormField", new { id = formField.FormFieldId }, formField);
    }

    // GET: api/FormFields/5
    [HttpGet("{id}")]
    public async Task<ActionResult<FormField>> GetFormField(int id)
    {
        var formField = await _context.FormFields.FindAsync(id);

        if (formField == null)
        {
            return NotFound();
        }

        return formField;
    }

    // PUT: api/FormFields/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutFormField(int id, FormField formField)
    {
        if (id != formField.FormFieldId)
        {
            return BadRequest();
        }

        _context.Entry(formField).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!FormFieldExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/FormFields/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFormField(int id)
    {
        var formField = await _context.FormFields.FindAsync(id);
        if (formField == null)
        {
            return NotFound();
        }

        _context.FormFields.Remove(formField);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/FormFields/reorder
    [HttpPost("reorder")]
    public async Task<IActionResult> ReorderFields([FromBody] List<int> fieldIds)
    {
        for (int i = 0; i < fieldIds.Count; i++)
        {
            var field = await _context.FormFields.FindAsync(fieldIds[i]);
            if (field != null)
            {
                field.DisplayOrder = i + 1;
            }
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    private bool FormFieldExists(int id)
    {
        return _context.FormFields.Any(e => e.FormFieldId == id);
    }
}