using Castle.Core.Configuration;
using CC.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Xunit;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace CC.Tests.Infrastructure
{
    public class EmailServiceTests
    {
        [Fact]
        public async Task SendEmailAsync_WithValidConfig_ReturnsTrue()
        {
            // 1. ARRANGE - Configuración manual para el test
            // Reemplaza estos valores con los de tu Sandbox de Mailtrap
            var inMemorySettings = new Dictionary<string, string> {
                {"SmtpSettings:Server", "sandbox.smtp.mailtrap.io"},
                {"SmtpSettings:Port", "587"},
                {"SmtpSettings:SenderName", "CC System"},
                {"SmtpSettings:SenderEmail", "slenderstalin@gmail.com"},
                {"SmtpSettings:Username", "b6e8a757c211a6"},
                {"SmtpSettings:Password", "13f774345f1e81"},
                {"SmtpSettings:EnableSsl", "true"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var emailService = new EmailService(configuration);

            // 2. ACT
            var result = await emailService.SendEmailAsync(
                "cepedayupanqui.kevin@gmail.com",
                "Prueba de Integración",
                "<h1>Hola!</h1><p>Este es un test del servicio de correos.</p>"
            );

            // 3. ASSERT
            Assert.True(result);
        }

        [Fact]
        public async Task SendConfirmationEmailAsync_SendsFormattedEmail()
        {
            // Similar al anterior, pero probando el método de confirmación
            var inMemorySettings = new Dictionary<string, string> {
                {"SmtpSettings:Server", "sandbox.smtp.mailtrap.io"},
                {"SmtpSettings:Port", "587"},
                {"SmtpSettings:SenderName", "CC System"},
                {"SmtpSettings:SenderEmail", "slenderstalin@gmail.com"},
                {"SmtpSettings:Username", "b6e8a757c211a6"},
                {"SmtpSettings:Password", "13f774345f1e81"}
            };

            var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            var emailService = new EmailService(configuration);

            // Act
            var result = await emailService.SendConfirmationEmailAsync("cepedayupanqui.kevin@gmail.com", "Juan Pérez", "token-de-prueba-123");

            // Assert
            Assert.True(result);
        }
    }
}
