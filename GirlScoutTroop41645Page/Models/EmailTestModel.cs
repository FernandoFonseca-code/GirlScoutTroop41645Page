using System.ComponentModel.DataAnnotations;

namespace GirlScoutTroop41645Page.Models;

public class EmailTestModel
{
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress]
    [Display(Name = "To Email")]
    public string ToEmail { get; set; } = string.Empty;

    [Display(Name = "Subject")]
    public string Subject { get; set; } = string.Empty;

    [Display(Name = "Message")]
    public string Message { get; set; } = string.Empty;

    public string ResultMessage { get; set; } = string.Empty;
}
