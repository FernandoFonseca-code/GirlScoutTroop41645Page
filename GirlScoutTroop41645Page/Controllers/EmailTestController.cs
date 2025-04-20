using GirlScoutTroop41645Page.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GirlScoutTroop41645Page.Controllers;

public class EmailTestController : Controller
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<EmailTestController> _logger;

    public EmailTestController(IEmailSender emailSender, ILogger<EmailTestController> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    // GET: Email/Test
    public IActionResult Test()
    {
        var model = new EmailTestModel
        {
            Subject = "Test Email from Girl Scout Troop 41645",
            Message = "<p>This is a <strong>test email</strong> from the Girl Scout Troop application.</p>"
        };

        return View(model);
    }

    // POST: Email/Test
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Test(EmailTestModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _emailSender.SendEmailAsync(
                model.ToEmail,
                model.Subject,
                model.Message);

            model.ResultMessage = $"Email sent successfully to {model.ToEmail}!";
            _logger.LogInformation("Test email sent to {Email}", model.ToEmail);
        }
        catch (Exception ex)
        {
            model.ResultMessage = $"Error sending email: {ex.Message}";
            _logger.LogError(ex, "Error sending test email to {Email}", model.ToEmail);
        }

        return View(model);
    }
}
