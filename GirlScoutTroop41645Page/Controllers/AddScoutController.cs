using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GirlScoutTroop41645Page.Models;
using GirlScoutTroop41645Page.Data;

namespace GirlScoutTroop41645Page.Controllers;

[Authorize(Roles = "Admin,TroopLeader,TroopSectionLeader")]
public class AddScoutController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AddScoutController> _logger;

    public AddScoutController(ApplicationDbContext context, ILogger<AddScoutController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var scouts = _context.Scouts.ToList();
        return View(scouts);
    }
}