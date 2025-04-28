using System.ComponentModel.DataAnnotations;

namespace GirlScoutTroop41645Page.Models;

public class CalendarEventViewModel
{
    [Required]
    [Display(Name = "Event Title")]
    public string Title { get; set; }

    [Display(Name = "Location")]
    public string Location { get; set; }

    [Display(Name = "Description")]
    public string Description { get; set; }

    [Required]
    [Display(Name = "Start Date/Time")]
    public DateTime StartDateTime { get; set; } = DateTime.Now.AddHours(1);

    [Required]
    [Display(Name = "End Date/Time")]
    public DateTime EndDateTime { get; set; } = DateTime.Now.AddHours(2);
}
