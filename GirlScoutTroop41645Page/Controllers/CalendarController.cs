using GirlScoutTroop41645Page.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GirlScoutTroop41645Page.Controllers;

[Authorize] // This attribute ensures only logged-in users can access
public class CalendarController : Controller
{
    public IActionResult Index()
    {
        if (User.IsInRole(IdentityHelper.TroopLeader))
        {
            // For Troop Leaders - show calendar with full editing capabilities
            ViewBag.CalendarUrl = "https://calendar.google.com/calendar/embed?height=600&wkst=1&ctz=America%2FLos_Angeles&showPrint=0&mode=WEEK&title=GS%20Troop%2041645&src=Z3N0cm9vcDQxNjQ1QGdtYWlsLmNvbQ&src=ZW4udXNhI2hvbGlkYXlAZ3JvdXAudi5jYWxlbmRhci5nb29nbGUuY29t&color=%23039BE5&color=%230B8043";
        }
        else if (User.IsInRole(IdentityHelper.TroopSectionLeader))
        {
            // For Section Leaders - show calendar with limited editing
            ViewBag.CalendarUrl = "https://calendar.google.com/calendar/embed?height=600&wkst=1&ctz=America%2FLos_Angeles&showPrint=0&mode=AGENDA&title=GS%20Troop%2041645&src=Z3N0cm9vcDQxNjQ1QGdtYWlsLmNvbQ&src=ZW4udXNhI2hvbGlkYXlAZ3JvdXAudi5jYWxlbmRhci5nb29nbGUuY29t&color=%23039BE5&color=%230B8043";
        }
        else
        {
            // For Parents - read-only view
            ViewBag.CalendarUrl = "https://calendar.google.com/calendar/embed?height=600&wkst=1&ctz=America%2FLos_Angeles&showPrint=0&mode=MONTH&title=GS%20Troop%2041645&src=Z3N0cm9vcDQxNjQ1QGdtYWlsLmNvbQ&src=ZW4udXNhI2hvbGlkYXlAZ3JvdXAudi5jYWxlbmRhci5nb29nbGUuY29t&color=%23039BE5&color=%230B8043";
        }
        return View();
    }
}
