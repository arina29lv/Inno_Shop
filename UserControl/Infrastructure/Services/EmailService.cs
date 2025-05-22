using System.Net;
using System.Net.Mail;
using UserControl.Infrastructure.Interfaces;

namespace UserControl.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public Task SendConfirmationEmailAsync(string toEmail, string confirmationLink)
    {
        var subject = "Confirm your email";
        var htmlBody = BuildEmailHtml("Welcome to Our App!", "Click the link below to confirm your email:", confirmationLink);
        return SendEmailAsync(toEmail, subject, htmlBody);
    }

    public Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        var subject = "Password Reset Request";
        var htmlBody = BuildEmailHtml("Reset your password", "Click the link below to reset your password:", resetLink);
        return SendEmailAsync(toEmail, subject, htmlBody);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var smtpSection = _config.GetSection("Smtp");

        var fromEmail = smtpSection["From"];
        var host = smtpSection["Host"];
        var port = int.Parse(smtpSection["Port"]);
        var enableSsl = bool.Parse(smtpSection["EnableSsl"]);
        var username = smtpSection["Username"];
        var password = smtpSection["Password"];

        using var smtpClient = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = enableSsl
        };

        var message = new MailMessage
        {
            From = new MailAddress(fromEmail!),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        message.To.Add(toEmail);
        await smtpClient.SendMailAsync(message);
    }

    private string BuildEmailHtml(string title, string message, string link)
    {
        return $"""
                <h3>{title}</h3>
                <p>{message}</p>
                <p><a href="{link}">Click here</a></p>
                <p>If the button doesn't work, copy this URL:</p>
                <p>{link}</p>
                """;
    }
    
}