using GirlScoutTroop41645Page.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GirlScoutTroop41645Page.Controllers;

public class EmailController : Controller
{
    private readonly UserManager<Member> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<EmailController> _logger;

    public EmailController(UserManager<Member> userManager, IEmailSender emailSender, ILogger<EmailController> logger)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _logger = logger;
    }

    // GET: Email/Send
    public IActionResult Send()
    {
        var model = new EmailModel
        {
            AvailableEmails = GetUserEmails(),
            Subject = "Write your Email Subject",
            Message = "Write your message here"
        };

        return View(model);
    }

    // POST: Email/Send
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(EmailModel model)
    {
        try
        {
            if (model.ToEmails == null || !model.ToEmails.Any())
            {
                model.ResultMessage = "No recipient/s selected.";
                model.AvailableEmails = GetUserEmails();
                return View(model);
            }

            // Option 1: Send individual emails to each recipient
            foreach (var recipientEmail in model.ToEmails)
            {
                await _emailSender.SendEmailAsync(
                    recipientEmail,
                    model.Subject,
                    model.Message);

                _logger.LogInformation("Email sent to {RecipientEmail}", recipientEmail);
            }

            model.ResultMessage = $"Email sent successfully to {model.ToEmails.Count} recipients!";
        }
        catch (Exception ex)
        {
            model.ResultMessage = $"Error sending email: {ex.Message}";
            _logger.LogError(ex, "Error sending email to multiple recipients");
        }

        model.AvailableEmails = GetUserEmails();
        return View(model);
    }

    private List<SelectListItem> GetUserEmails()
    {
        var users = _userManager.Users.ToList();
        return users.Select(u => new SelectListItem
        {
            Value = u.Email,
            Text = $"{u.FirstName} {u.LastName} ({u.Email})"
        }).ToList();

    }
}
