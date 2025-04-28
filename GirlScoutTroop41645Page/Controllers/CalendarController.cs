using GirlScoutTroop41645Page.Models;
using GirlScoutTroop41645Page.Services;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GirlScoutTroop41645Page.Controllers;

[Authorize] // This attribute ensures only logged-in users can access
public class CalendarController : Controller
{
    private readonly GoogleCalendarService _googleCalendarService;

    public CalendarController(GoogleCalendarService googleCalendarService)
    {
        _googleCalendarService = googleCalendarService;
    }
    public async Task<IActionResult> Index()
    {
        try
        {
            var service = await _googleCalendarService.GetCalendarServiceAsync();

            // If service is null, we've been redirected to authorization
            if (service == null) return new EmptyResult();

            var events = await GetUpcomingEventsAsync(service);
            return View(events);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Calendar error: {ex.Message}";
            return View(new List<Event>());
        }
    }

    private async Task<List<Event>> GetUpcomingEventsAsync(Google.Apis.Calendar.v3.CalendarService service)
    {
        string calendarId = _googleCalendarService.GetCalendarId();
        
        // Define parameters of the request
        var request = service.Events.List(calendarId); 
        request.TimeMinDateTimeOffset = DateTime.Now;
        request.ShowDeleted = false;
        request.SingleEvents = true;
        request.MaxResults = 10;
        request.OrderBy = Google.Apis.Calendar.v3.EventsResource.ListRequest.OrderByEnum.StartTime;

        // Fetch events
        var events = await request.ExecuteAsync();
        return events.Items.ToList();
    }

    [Authorize(Roles = "TroopLeader,TroopSectionLeader")]
    public IActionResult Create()
    {
        return View(new CalendarEventViewModel());
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

        try
        {
            var service = await _googleCalendarService.GetCalendarServiceAsync();

            var newEvent = new Event
            {
                Summary = model.Title,
                Location = model.Location,
                Description = model.Description,
                Start = new EventDateTime
                {
                    DateTimeDateTimeOffset = model.StartDateTime,
                    TimeZone = "America/Los_Angeles"
                },
                End = new EventDateTime
                {
                    DateTimeDateTimeOffset = model.EndDateTime,
                    TimeZone = "America/Los_Angeles"
                }
            };

            // Add event to primary calendar
            var createdEvent = await service.Events.Insert(newEvent, _googleCalendarService.GetCalendarId()).ExecuteAsync();

            TempData["SuccessMessage"] = "Event created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error creating event: " + ex.Message);
            return View(model);
        }
    }
}
