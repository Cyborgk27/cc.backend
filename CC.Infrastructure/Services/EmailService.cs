using CC.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace CC.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config) => _config = config;

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_config["SmtpSettings:SenderName"], _config["SmtpSettings:SenderEmail"]));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = body };
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_config["SmtpSettings:Server"], int.Parse(_config["SmtpSettings:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_config["SmtpSettings:Username"], _config["SmtpSettings:Password"]);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SendConfirmationEmailAsync(string to, string userName, string token)
        {
            // Aquí puedes usar una plantilla HTML más elaborada
            string body = $@"
                <h1>Bienvenido, {userName}</h1>
                <p>Gracias por registrarte. Por favor confirma tu cuenta haciendo clic en el siguiente enlace:</p>
                <a href='https://tuapp.com/confirm?token={token}&email={to}'>Confirmar Cuenta</a>";

            return await SendEmailAsync(to, "Confirma tu cuenta - CC System", body);
        }
    }
}