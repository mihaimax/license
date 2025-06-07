using Microsoft.Extensions.Configuration;
namespace StudentPortal.Classes
{
    public class MailManager
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        public MailManager(IConfiguration configuration)
        {
            _smtpServer = "smtp.gmail.com";
            _smtpPort = 587;
            _smtpUser = configuration["Smtp:Email"];
            _smtpPass = configuration["Smtp:Password"];
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new System.Net.Mail.SmtpClient(_smtpServer, _smtpPort))
            {
                client.Credentials = new System.Net.NetworkCredential(_smtpUser, _smtpPass);
                client.EnableSsl = true;
                var mailMessage = new System.Net.Mail.MailMessage
                {
                    From = new System.Net.Mail.MailAddress(_smtpUser),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
