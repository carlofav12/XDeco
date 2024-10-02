using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace XDeco.Service
{
    public class EmailService
    {
        private readonly SmtpClient _smtpClient;

        public EmailService(IConfiguration configuration)
        {
            _smtpClient = new SmtpClient
            {
                Host = configuration["MailSettings:Host"],
                Port = int.Parse(configuration["MailSettings:Port"]),
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    configuration["MailSettings:Username"],
                    configuration["MailSettings:Password"]
                )
            };
        }

        public void SendEmail(string to, string subject, string body)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpClient.Credentials.GetCredential(_smtpClient.Host, _smtpClient.Port, "basic").UserName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

            // Aquí enviamos el correo
            _smtpClient.Send(mailMessage);

            // Aquí imprimimos en consola el correo enviado
            Console.WriteLine($"Email enviado a: {to}");
            Console.WriteLine($"Asunto: {subject}");
            Console.WriteLine($"Contenido: {body}");
        }
    }
}
