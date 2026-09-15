using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace ProblemTalepTakipSistemiHalkbank.Services
{
    public class EmailServisi : IEmailServisi
    {
        private readonly IConfiguration _configuration;

        public EmailServisi(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            // appsettings.json dosyasından SMTP ayarlarını okur
            var smtpServer = _configuration["Smtp:Server"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "587");
            var smtpUser = _configuration["Smtp:User"];
            var smtpPass = _configuration["Smtp:Password"];

            var mail = new MailMessage
            {
                From = new MailAddress(smtpUser ?? "noreply@halkbank.com.tr", "Halkbank Problem Takip Sistemi"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true,
                UseDefaultCredentials = false, // BU SATIR ÇOK ÖNEMLİ (Credentials'ın ezilmesini önler)
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            await client.SendMailAsync(mail);
        }
    }
}