using CC.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

                smtp.AuthenticationMechanisms.Remove("XOAUTH2"); // Gmail a veces lo pide, pero tú usas App Password
                smtp.AuthenticationMechanisms.Remove("GSSAPI"); // ESTA ES LA QUE DA EL ERROR
                smtp.AuthenticationMechanisms.Remove("NTLM");

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

        public async Task<bool> SendConfirmationEmailAsync(string to, string userName)
        {
            string body = $@"
                <div style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; background-color: #f4f7f6; padding: 50px 20px; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 20px; overflow: hidden; box-shadow: 0 15px 35px rgba(0,0,0,0.05);'>
            
                        <div style='background-color: #f8fafc; padding: 40px; text-align: center; border-bottom: 1px solid #edf2f7;'>
                            <div style='font-size: 50px; margin-bottom: 15px;'>⏳</div>
                            <h1 style='color: #1e293b; margin: 0; font-size: 24px; font-weight: 700;'>Solicitud en Revisión</h1>
                            <p style='color: #64748b; margin-top: 5px; font-size: 14px;'>ID de Usuario: {userName}</p>
                        </div>

                        <div style='padding: 40px;'>
                            <h2 style='color: #334155; margin-top: 0; font-size: 18px;'>Hola, {userName}</h2>
                            <p style='color: #475569; font-size: 16px; line-height: 1.7;'>
                                Hemos recibido tu solicitud para unirte a <strong>CC System</strong>. Para garantizar la seguridad de nuestra plataforma, todas las cuentas nuevas deben ser validadas por nuestro equipo administrativo.
                            </p>

                            <div style='background-color: #f0f9ff; border: 1px solid #bae6fd; border-radius: 12px; padding: 20px; margin: 30px 0; text-align: center;'>
                                <p style='margin: 0; color: #0369a1; font-weight: 600; font-size: 15px;'>
                                    Estado actual: <span style='background-color: #e0f2fe; padding: 4px 10px; border-radius: 20px;'>Pendiente de Aprobación</span>
                                </p>
                            </div>

                            <p style='color: #64748b; font-size: 15px;'>
                                <strong>¿Qué sigue ahora?</strong><br>
                                Nuestro administrador revisará tus datos. Una vez aprobada la cuenta, recibirás un correo electrónico con tus credenciales y el enlace de acceso.
                            </p>

                            <div style='margin-top: 30px; border-top: 1px solid #f1f5f9; padding-top: 20px;'>
                                <p style='color: #94a3b8; font-size: 13px; text-align: center;'>
                                    No es necesario que realices ninguna acción adicional por el momento.
                                </p>
                            </div>
                        </div>

                        <div style='background-color: #1e293b; padding: 20px; text-align: center;'>
                            <p style='margin: 0; font-size: 12px; color: #94a3b8;'>© 2026 CC System. Todos los derechos reservados.</p>
                        </div>
                    </div>
                </div>";

            return await SendEmailAsync(to, "Hemos recibido tu registro - CC System", body);
        }
        public async Task<bool> SendLoginNotificationEmailAsync(string to, string userName)
        {
            // Obtenemos la fecha y hora actual para darle más credibilidad al aviso
            string fechaHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            string body = $@"
                <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7f9; padding: 40px 10px; line-height: 1.6;'>
                    <div style='max-width: 500px; margin: 0 auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1); border: 1px solid #e1e8ed;'>
            
                        <div style='background-color: #1a73e8; padding: 30px; text-align: center;'>
                            <div style='font-size: 50px; margin-bottom: 10px;'>🛡️</div>
                            <h1 style='color: #ffffff; margin: 0; font-size: 22px; font-weight: 600;'>Alerta de Seguridad</h1>
                        </div>

                        <div style='padding: 30px; color: #3c4043;'>
                            <h2 style='margin-top: 0; color: #202124; font-size: 18px;'>Hola, {userName}</h2>
                            <p style='font-size: 15px;'>Se ha detectado un <strong>nuevo inicio de sesión</strong> en tu cuenta de <strong>CC System</strong>.</p>
                
                            <div style='background-color: #f8f9fa; border-left: 4px solid #1a73e8; padding: 15px; margin: 20px 0;'>
                                <p style='margin: 0; font-size: 14px; color: #5f6368;'><strong>Fecha y hora:</strong> {fechaHora}</p>
                                <p style='margin: 5px 0 0 0; font-size: 14px; color: #5f6368;'><strong>Ubicación:</strong> Detectada vía IP</p>
                            </div>
                        </div>

                        <div style='background-color: #f1f3f4; padding: 20px; text-align: center; font-size: 12px; color: #70757a;'>
                            <p style='margin: 0;'>Este es un mensaje automático de CC System.</p>
                            <p style='margin: 5px 0 0 0;'>Para proteger tu privacidad, no compartas este correo con nadie.</p>
                        </div>
                    </div>
                </div>";

            return await SendEmailAsync(to, "⚠️ Alerta de Inicio de Sesión - CC System", body);
        }

        public async Task<bool> SendNotificationActiveAccount(string to, string username)
        {
            string loginUrl = "https://ccfrontend-production-307f.up.railway.app/auth/sign-in"; // Ajusta a tu URL real

            string body = $@"
                <div style='font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; background-color: #f8fafc; padding: 50px 20px; color: #1e293b;'>
                    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 24px; overflow: hidden; box-shadow: 0 20px 40px rgba(0,0,0,0.08);'>
            
                        <div style='background: linear-gradient(135deg, #10b981 0%, #059669 100%); padding: 50px; text-align: center;'>
                            <div style='background-color: rgba(255,255,255,0.2); width: 80px; height: 80px; border-radius: 50%; margin: 0 auto 20px; display: flex; align-items: center; justify-content: center;'>
                                <span style='font-size: 40px;'>✅</span>
                            </div>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: 800; letter-spacing: -0.5px;'>¡Cuenta Activada!</h1>
                        </div>

                        <div style='padding: 40px; text-align: center;'>
                            <h2 style='color: #334155; margin-top: 0; font-size: 22px;'>¡Buenas noticias, {username}!</h2>
                            <p style='color: #64748b; font-size: 17px; line-height: 1.8; margin-bottom: 30px;'>
                                Tu cuenta en <strong>CC System</strong> ha sido revisada y aprobada por nuestro equipo administrativo. Ya tienes acceso total a todas nuestras funciones.
                            </p>

                            <div style='margin: 40px 0;'>
                                <a href='{loginUrl}' style='background-color: #10b981; color: #ffffff; padding: 16px 45px; text-decoration: none; border-radius: 12px; font-weight: bold; font-size: 18px; display: inline-block; box-shadow: 0 10px 15px -3px rgba(16, 185, 129, 0.3); transition: all 0.3s ease;'>
                                    Ingresar al Sistema
                                </a>
                            </div>

                            <div style='background-color: #f1f5f9; border-radius: 12px; padding: 20px; margin-top: 20px;'>
                                <p style='margin: 0; color: #475569; font-size: 14px;'>
                                    <strong>Sugerencia:</strong> Te recomendamos marcar nuestra página en tus favoritos para un acceso más rápido.
                                </p>
                            </div>
                        </div>

                        <div style='background-color: #f8fafc; padding: 30px; text-align: center; border-top: 1px solid #f1f5f9;'>
                            <p style='margin: 0; font-size: 13px; color: #94a3b8;'>Si tienes problemas para ingresar, contacta a soporte técnico.</p>
                            <p style='margin: 10px 0 0 0; font-size: 13px; color: #94a3b8; font-weight: bold;'>© 2026 CC System</p>
                        </div>
                    </div>
                </div>";

            return await SendEmailAsync(to, "¡Felicidades! Tu cuenta ha sido activada - CC System", body);
        }
    }
}