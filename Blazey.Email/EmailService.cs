using System.Net.Mail;
using System.Net;

namespace Blazey.Email;

public class EmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(EmailSettings emailSettings)
    {
        _emailSettings = emailSettings;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        using (var client = new SmtpClient(_emailSettings.MailServer, _emailSettings.MailPort))
        {
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password);
            client.EnableSsl = true;  // Update this according to your mail server settings

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
}