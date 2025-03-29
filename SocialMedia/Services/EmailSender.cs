using Microsoft.AspNetCore.Identity.UI.Services;

namespace SocialMedia.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine($"Sending Email to {email}: {subject}");
            return Task.CompletedTask;
        }
    }
}
