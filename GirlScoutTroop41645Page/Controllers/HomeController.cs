using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GirlScoutTroop41645Page.Models;
using Microsoft.AspNetCore.Authorization;

namespace GirlScoutTroop41645Page.Controllers;
/// <summary>
/// Rest of website is secured behind login credentials
/// </summary>
[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    // Can view home page without a login (PUBLIC)
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}