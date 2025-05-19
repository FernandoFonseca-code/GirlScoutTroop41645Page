using GirlScoutTroop41645Page.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GirlScoutTroop41645Page.Controllers;

public class EmailController : Controller
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<EmailController> _logger;

    public EmailController(IEmailSender emailSender, ILogger<EmailController> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    // GET: SendEmail/SendEmail
    public IActionResult SendEmail()
    {
        var model = new EmailModel
        {
            Subject = "Write your Email Subject",
            Message = "Write your message here"
        };

        return View(model);
    }

    // POST: SendEmail/SendEmail
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Test(EmailModel model)
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

            model.ResultMessage = $"SendEmail sent successfully to {model.ToEmail}!";
            _logger.LogInformation("SendEmail email sent to {SendEmail}", model.ToEmail);
        }
        catch (Exception ex)
        {
            model.ResultMessage = $"Error sending email: {ex.Message}";
            _logger.LogError(ex, "Error sending test email to {SendEmail}", model.ToEmail);
        }

        return View(model);
    }
}
