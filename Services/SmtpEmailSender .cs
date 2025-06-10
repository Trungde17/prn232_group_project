using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var email = new MailMessage();
            email.From = new MailAddress(_config["Email:From"]);
            email.To.Add(toEmail);
            email.Subject = subject;
            email.Body = htmlMessage;
            email.IsBodyHtml = true;

            using var client = new SmtpClient(_config["Email:Smtp"], int.Parse(_config["Email:Port"]))
            {
                Credentials = new NetworkCredential(_config["Email:From"], _config["Email:Password"]),
                EnableSsl = true
            };

            await client.SendMailAsync(email);
        }
    }
}
