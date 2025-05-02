using GirlScoutTroop41645Page.Models;
using GirlScoutTroop41645Page.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Calendar.v3.Data;
using Microsoft.Extensions.Logging;

namespace GirlScoutTroop41645Page.Controllers;

[Authorize]
public class CalendarController : Controller
{
    private readonly GoogleCalendarService _googleCalendarService;
    private readonly ILogger<CalendarController> _logger;
    private readonly string _timeZone;

    public CalendarController(
        GoogleCalendarService googleCalendarService,
        ILogger<CalendarController> logger,
        IConfiguration configuration)
    {
        _googleCalendarService = googleCalendarService;
        _logger = logger;
        _timeZone = configuration["GoogleCalendar:TimeZone"] ?? "America/Los_Angeles";
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var service = await _googleCalendarService.GetCalendarServiceAsync();

            // If service is null, we've been redirected to authorization
            if (service == null) return new EmptyResult();

            var events = await _googleCalendarService.GetUpcomingEventsAsync(10);
            return View(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading calendar events");
            TempData["ErrorMessage"] = $"Calendar error: {ex.Message}";
            return View(new List<Event>());
        }
    }

    [Authorize(Roles = "TroopLeader,TroopSectionLeader")]
    public IActionResult Create()
    {
        return View(new CalendarEventViewModel
        {
            StartDateTime = DateTime.Now.AddHours(1),
            EndDateTime = DateTime.Now.AddHours(2)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TroopLeader,TroopSectionLeader")]
    public async Task<IActionResult> Create(CalendarEventViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.EndDateTime <= model.StartDateTime)
        {
            ModelState.AddModelError("EndDateTime", "End time must be after start time");
            return View(model);
        }

        try
        {
            var service = await _googleCalendarService.GetCalendarServiceAsync();
            if (service == null) return new EmptyResult();

            var newEvent = MapToEvent(model);
            var createdEvent = await _googleCalendarService.CreateEventAsync(newEvent);

            TempData["SuccessMessage"] = "Event created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating calendar event");
            ModelState.AddModelError("", "Error creating event: " + ex.Message);
            return View(model);
        }
    }

    private Event MapToEvent(CalendarEventViewModel model)
    {
        return new Event
        {
            Summary = model.Title,
            Location = model.Location,
            Description = model.Description,
            Start = new EventDateTime
            {
                DateTimeDateTimeOffset = model.StartDateTime,
                TimeZone = _timeZone
            },
            End = new EventDateTime
            {
                DateTimeDateTimeOffset = model.EndDateTime,
                TimeZone = _timeZone
            }
        };
    }
}