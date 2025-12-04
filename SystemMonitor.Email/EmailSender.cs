
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Resend;
using SystemMonitor.Application.Abstractions.Services;
using SystemMonitor.Configuration.Options;

namespace SystemMonitor.Email;

public class EmailSender(IOptions<EmailOptions> options, IResend resend) : IEmailSender, IEmailService
{
    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        var email = new EmailMessage
        {
            From = options.Value.From,
            To = [ toEmail ],
            Subject = subject,
            HtmlBody = message
        };

        await resend.EmailSendAsync(email);
    }
}
