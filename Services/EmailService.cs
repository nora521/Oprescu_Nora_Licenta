using Licenta.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

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

            email.From.Add(new MailboxAddress(_config["EmailSettings:SenderName"], _config["EmailSettings:SenderEmail"]));
            email.To.Add(new MailboxAddress(toName, toEmail));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; 
            try
            {

                await smtp.ConnectAsync(_config["EmailSettings:SmtpServer"],
                                      int.Parse(_config["EmailSettings:Port"]),
                                      MailKit.Security.SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(_config["EmailSettings:SenderEmail"], _config["EmailSettings:Password"]);

                await smtp.SendAsync(email);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }

        public async Task TrimiteEmailRezervareAsync(string toEmail, string toName, Rezervare rezervare)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_config["EmailSettings:SenderName"], _config["EmailSettings:SenderEmail"]));
            email.To.Add(new MailboxAddress(toName, toEmail));
            email.Subject = "Confirmare rezervare";

            string html = $@"
        <div style='font-family: Arial; padding: 20px;'>
            <h2>Confirmare rezervare</h2>
            <p>Bună <b>{toName}</b>,</p>
            <p>Rezervarea ta a fost înregistrată cu succes.</p>

            <h3>Detalii rezervare</h3>
            <ul>
                <li><b>Vehicul:</b> {rezervare.Autovehicul.Marca.NumeMarca} {rezervare.Autovehicul.Model}</li>
                <li><b>Data început:</b> {rezervare.DataStart:dd.MM.yyyy}</li>
                <li><b>Data sfârșit:</b> {rezervare.DataFinal:dd.MM.yyyy}</li>
                <li><b>Preț/zi:</b> {rezervare.PretZi} (€)</li>
                <li><b>Număr zile:</b> {(rezervare.DataFinal - rezervare.DataStart).Days}</li>
                <li><b>Garanție:</b> {rezervare.Garantie} €</li>
                <li><b>Preț total:</b> {rezervare.PretTotal} €</li>
            </ul>
        </div>";

            var bodyBuilder = new BodyBuilder { HtmlBody = html };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await smtp.ConnectAsync(_config["EmailSettings:SmtpServer"],
                                    int.Parse(_config["EmailSettings:Port"]),
                                    MailKit.Security.SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(_config["EmailSettings:SenderEmail"], _config["EmailSettings:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task TrimiteEmailFeedbackAsync(string toEmail, string toName, int rezervareId)
        {
            string baseUrl = "https://localhost:7028";

            string feedbackLink = $"{baseUrl}/Feedbacks/Create?rezervareId={rezervareId}";

            string html = $@"
    <div style='font-family: Arial; padding: 20px;'>
        <h2>Îți mulțumim pentru rezervare </h2>

        <p>Bună <b>{toName}</b>,</p>

        <p>Perioada ta de închiriere s-a încheiat.</p>

        <p>Ne-ar ajuta enorm dacă ai lăsa un feedback despre experiența avută.</p>

        <br/>

        <a href='{feedbackLink}'
           style='background-color:#007bff;
                  color:white;
                  padding:12px 20px;
                  text-decoration:none;
                  border-radius:8px;
                  font-weight:bold;'>
            Oferă feedback
        </a>

        <br/><br/>

        <p> Vă mulțumim!</p>
    </div>";

            await SendNotificationEmailAsync(
                toEmail,
                toName,
                "Cum a fost experiența ta?",
                html
            );
        }


    }
}
