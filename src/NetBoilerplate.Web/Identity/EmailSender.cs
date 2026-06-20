using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using NetBoilerplate.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace NetBoilerplate.Web.Identity;

public class EmailSender(IOptions<SmtpOptions> options) :
    IEmailSender<ApplicationUser>,
    ISmtpTestSender
{
    private readonly SmtpOptions _options = options.Value;

    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        return SendEmailAsync(email, "Confirm your account",
            $"Confirm your account by following <a href=\"{HtmlEncoder.Default.Encode(confirmationLink)}\">this link</a>.");
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        return SendEmailAsync(email, "Reset your password",
            $"Reset your password by following <a href=\"{HtmlEncoder.Default.Encode(resetLink)}\">this link</a>.");
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        return SendEmailAsync(email, "Reset your password",
            $"Your password reset code is <strong>{HtmlEncoder.Default.Encode(resetCode)}</strong>.");
    }

    public Task SendTestAsync(string email, CancellationToken token = default)
    {
        return SendEmailAsync(email, "SMTP test",
            "SMTP is configured correctly. This is a test email.", token);
    }

    private async Task SendEmailAsync(
        string email,
        string subject,
        string body,
        CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Host) || string.IsNullOrWhiteSpace(_options.SenderEmail))
        {
            throw new InvalidOperationException("SMTP host and sender email must be configured.");
        }

        if (string.IsNullOrWhiteSpace(_options.UserName) != string.IsNullOrWhiteSpace(_options.Password))
        {
            throw new InvalidOperationException("SMTP username and password must be configured together.");
        }

        using var message = new MailMessage();
        message.From = new MailAddress(_options.SenderEmail, _options.SenderName);
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = true;
        message.To.Add(email);

        using var client = new SmtpClient(_options.Host, _options.Port);
        client.EnableSsl = _options.EnableSsl;

        if (!string.IsNullOrWhiteSpace(_options.UserName))
        {
            client.Credentials = new NetworkCredential(_options.UserName, _options.Password);
        }

        await client.SendMailAsync(message, token);
    }
}
