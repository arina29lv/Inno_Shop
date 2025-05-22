namespace UserControl.Infrastructure.Interfaces;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string toEmail, string confirmationLink);
    Task SendPasswordResetEmailAsync(string email, string resetLink);
}