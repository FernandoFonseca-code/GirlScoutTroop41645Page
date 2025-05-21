using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GirlScoutTroop41645Page.Models;

public class EmailModel
{
    // This property is used to store the selected email address from the dropdown list.
    [Display(Name = "To")]
    public string To { get; set; } = string.Empty;
    
    //This holds the list of email addresses from the Users Db to which the email will be sent.
    [Required(ErrorMessage = "At least one recipient is required")]
    [Display(Name = "Recipients")]
    public List<string> ToEmails { get; set; } = new List<string>();

    public List<SelectListItem>? AvailableEmails { get; set; }

    [Display(Name = "Subject")]
    public string Subject { get; set; } = string.Empty;

    [Display(Name = "Message")]
    public string Message { get; set; } = string.Empty;

    public string ResultMessage { get; set; } = string.Empty;
}
