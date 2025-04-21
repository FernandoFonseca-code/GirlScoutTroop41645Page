using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace GirlScoutTroop41645Page.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        string apiKey = _configuration["ApiSettings:ApiKey"];
        string sendGrid_Sender = _configuration["SendGrid_Sender:senderEmail"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("SendGrid API Key is null or empty");
        }

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(sendGrid_Sender, "noreply@Girl Scout Troop 41645");
        var to = new EmailAddress(toEmail);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: "testing this process", htmlMessage);

        var response = await client.SendEmailAsync(msg);

        _logger.LogInformation(response.IsSuccessStatusCode
            ? $"Email to {toEmail} queued successfully!"
            : $"Failed to send email to {toEmail}");
    }
}
