using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace Licenta.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendNotificationEmailAsync(string toEmail, string toName, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            // Expeditorul  (admin)
            email.From.Add(new MailboxAddress(_config["EmailSettings:SenderName"], _config["EmailSettings:SenderEmail"]));
            // Destinatarul 
            email.To.Add(new MailboxAddress(toName, toEmail));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; //orice certificat il face valid 
            try
            {
                // Conectare la yahoo
                await smtp.ConnectAsync(_config["EmailSettings:SmtpServer"],
                                      int.Parse(_config["EmailSettings:Port"]),
                                      MailKit.Security.SecureSocketOptions.StartTls);

                // Autentificare 
                await smtp.AuthenticateAsync(_config["EmailSettings:SenderEmail"], _config["EmailSettings:Password"]);

                await smtp.SendAsync(email);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }

    }
}
