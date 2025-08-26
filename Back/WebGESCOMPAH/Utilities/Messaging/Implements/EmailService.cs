using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;
using Utilities.Messaging.Interfaces;

namespace Utilities.Messaging.Implements
{
    /// <summary>
    /// Servicio de envío de correos para GESCOMPAH
    /// </summary>
    public class EmailService : ISendCode
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService>? _logger;

        // 🎨 Branding GESCOMPAH
        private const string BrandName = "GESCOMPAH";
        private const string BrandPrimary = "#2E7D32";  // verde header (ya lo usas en el layout)
        private const string BrandAccent = "#16a34a";  // verde acento (toggle on)
        private const string BrandText = "#1f2937";  // gris oscuro legible
        private const string BrandMuted = "#6b7280";  // gris secundario
        private const string BrandBorder = "#e5e7eb";  // gris bordes

        public EmailService(IConfiguration config, ILogger<EmailService>? logger = null)
        {
            _config = config;
            _logger = logger;
        }

        // 📧 MÉTODO PRIVADO REUTILIZABLE
        private async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var emailEmisor = _config["CONFIGURACIONES_EMAIL:EMAIL"]!;
            var password = _config["CONFIGURACIONES_EMAIL:PASSWORD"];
            var host = _config["CONFIGURACIONES_EMAIL:HOST"];
            var puerto = int.Parse(_config["CONFIGURACIONES_EMAIL:PUERTO"]!);

            using var smtpCliente = new SmtpClient(host!, puerto)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailEmisor, password)
            };

            // Mostrar nombre amigable del remitente
            var displayName = $"{BrandName} • Soporte";
            var from = new MailAddress(emailEmisor, displayName, Encoding.UTF8);
            var toAddr = new MailAddress(to);

            using var mensaje = new MailMessage(from, toAddr)
            {
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };

            try
            {
                await smtpCliente.SendMailAsync(mensaje);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error enviando correo a {To} con asunto {Subject}", to, subject);
                throw; // deja que lo capture tu middleware global
            }
        }

        // 🧩 Plantilla base HTML (brand + layout)
        private string WrapEmail(string title, string contentHtml)
        {
            //var appUrl = _config["APP:FRONTEND_URL"] ?? "https://app.gescomph.com";
            var logoUrl = _config["APP:LOGO_URL"];

            var logoHtml = !string.IsNullOrWhiteSpace(logoUrl)
                ? $"<img src='{logoUrl}' alt='{BrandName}' style='height:40px; display:block;'/>"
                : $"<strong style='font-size:18px; color:white; letter-spacing:.5px'>{BrandName}</strong>";

            return $@"
                    <!DOCTYPE html>
                    <html lang='es'>
                    <head>
                      <meta charset='UTF-8'>
                      <meta name='viewport' content='width=device-width, initial-scale=1.0'/>
                      <title>{title}</title>
                    </head>
                    <body style='margin:0; padding:24px; background:#f8fafc; font-family:Segoe UI, Roboto, Arial, sans-serif;'>
                      <div style='max-width:640px; margin:0 auto;'>
                        <!-- Header -->
                        <div style='background:{BrandPrimary}; padding:16px 20px; border-radius:12px 12px 0 0;'>
                          {logoHtml}
                        </div>

                        <!-- Card -->
                        <div style='background:white; border:1px solid {BrandBorder}; border-top:none; padding:28px; border-radius:0 0 12px 12px; box-shadow:0 6px 24px rgba(0,0,0,.06);'>
                          <h2 style='margin:0 0 12px 0; color:{BrandText}; font-size:20px;'>{title}</h2>
                          <div style='color:{BrandText}; line-height:1.6; font-size:15px;'>{contentHtml}</div>

                          <hr style='border:none; border-top:1px solid {BrandBorder}; margin:24px 0'/>

                          <p style='margin:0; color:{BrandMuted}; font-size:12px;'>
                            Este mensaje fue enviado por {BrandName}. Si no reconoces esta solicitud, ignora este correo.
                          </p>
                        </div>
                      </div>
                    </body>
                    </html>";
        }

        // 🔄 MÉTODO 1: Enviar código de recuperación
        public async Task SendRecoveryCodeEmail(string emailReceptor, string recoveryCode)
        {
            var content = $@"
                        <p>Hemos recibido una solicitud para <strong>restablecer tu contraseña</strong>.</p>
                        <p>Tu código de verificación es:</p>
                        <div style='display:inline-block; padding:12px 18px; font-size:26px; font-weight:700; letter-spacing:2px; background:{BrandAccent}; color:white; border-radius:10px;'>
                            {recoveryCode}
                        </div>
                        <p style='margin-top:16px; color:{BrandMuted}; font-size:13px;'>
                            Este código es válido por <strong>10 minutos</strong>.
                        </p>";

            var html = WrapEmail("Recuperación de contraseña", content);
            await SendEmailAsync(emailReceptor, "GESCOMPAH – Recuperación de contraseña", html);
        }

        // 🔐 MÉTODO 2: Enviar contraseña temporal (forzar cambio en 1er login)
        public async Task SendTemporaryPasswordAsync(string email, string fullName, string tempPassword)
        {
            var appUrl = _config["APP:FRONTEND_URL"] ?? "https://app.gescomph.com";
            var content = $@"
                        <p>Hola <strong>{fullName}</strong>, tu cuenta en <strong>{BrandName}</strong> ha sido creada exitosamente.</p>
                        <p>Tu <strong>contraseña temporal</strong> es:</p>

                        <div style='display:inline-block; padding:12px 18px; font-size:22px; font-weight:700; background:#111827; color:white; border-radius:10px;'>
                            {tempPassword}
                        </div>

                        <p style='margin-top:16px;'>Por seguridad, deberás cambiarla en tu <strong>primer ingreso</strong> al sistema.</p>";

            var html = WrapEmail("Tu cuenta fue creada", content);
            await SendEmailAsync(email, "GESCOMPAH – Tu cuenta fue creada", html);
        }
    }
}
