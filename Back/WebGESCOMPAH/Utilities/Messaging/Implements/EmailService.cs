using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;
using Utilities.Messaging.Interfaces;

namespace Utilities.Messaging.Implements
{
    /// <summary>
    /// Servicio de envío de correos para GESCOMPAH (robustecido con validaciones).
    /// </summary>
    public class EmailService : ISendCode
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService>? _logger;

        // 🎨 Branding GESCOMPAH
        private const string BrandName = "GESCOMPAH";
        private const string BrandPrimary = "#2E7D32";  // verde header
        private const string BrandAccent = "#16a34a";   // verde acento
        private const string BrandText = "#1f2937";     // gris oscuro
        private const string BrandMuted = "#6b7280";    // gris secundario
        private const string BrandBorder = "#e5e7eb";   // gris bordes

        public EmailService(IConfiguration config, ILogger<EmailService>? logger = null)
        {
            _config = config;
            _logger = logger;
        }

        // ====== Helpers de validación ======

        private static void EnsureValidEmail(string? email, string paramName)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException($"El email '{paramName}' no puede ser nulo o vacío.", paramName);

            try { _ = new MailAddress(email); }
            catch
            {
                throw new ArgumentException($"El email '{paramName}' no tiene un formato válido.", paramName);
            }
        }

        private (string FromEmail, string Password, string Host, int Port, bool EnableSsl) LoadSmtpConfig()
        {
            var emailEmisor = _config["CONFIGURACIONES_EMAIL:EMAIL"];
            var password = _config["CONFIGURACIONES_EMAIL:PASSWORD"];
            var host = _config["CONFIGURACIONES_EMAIL:HOST"];
            var puertoStr = _config["CONFIGURACIONES_EMAIL:PUERTO"];
            var enableSslStr = _config["CONFIGURACIONES_EMAIL:ENABLE_SSL"]; // opcional

            if (string.IsNullOrWhiteSpace(emailEmisor))
                throw new InvalidOperationException("CONFIGURACIONES_EMAIL:EMAIL no está configurado.");

            if (string.IsNullOrWhiteSpace(host))
                throw new InvalidOperationException("CONFIGURACIONES_EMAIL:HOST no está configurado.");

            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("CONFIGURACIONES_EMAIL:PASSWORD no está configurado.");

            if (!int.TryParse(puertoStr, out var puerto) || puerto <= 0)
                throw new InvalidOperationException("CONFIGURACIONES_EMAIL:PUERTO no es un entero válido (> 0).");

            var enableSsl = true;
            if (!string.IsNullOrWhiteSpace(enableSslStr) && bool.TryParse(enableSslStr, out var parsed))
                enableSsl = parsed;

            EnsureValidEmail(emailEmisor, "CONFIGURACIONES_EMAIL:EMAIL");

            return (emailEmisor, password, host!, puerto, enableSsl);
        }

        // ====== Envío base ======
        private async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            // Validaciones de entrada
            EnsureValidEmail(to, nameof(to));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("El asunto no puede ser nulo o vacío.", nameof(subject));
            if (string.IsNullOrWhiteSpace(htmlBody))
                throw new ArgumentException("El cuerpo del correo no puede ser nulo o vacío.", nameof(htmlBody));

            // Carga y valida configuración SMTP
            var (fromEmail, password, host, port, enableSsl) = LoadSmtpConfig();

            using var smtpCliente = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail, password)
            };

            var displayName = $"{BrandName} • Soporte";
            var from = new MailAddress(fromEmail, displayName, Encoding.UTF8);
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
                _logger?.LogInformation("Email enviado a {To} con asunto {Subject}", to, subject);
            }
            catch (SmtpException ex)
            {
                _logger?.LogError(ex, "SMTP error enviando correo a {To} (host {Host}:{Port})", to, host, port);
                throw; // deja que lo capture el middleware global
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error general enviando correo a {To}", to);
                throw;
            }
        }

        // 🧩 Plantilla base HTML (brand + layout)
        private string WrapEmail(string title, string contentHtml)
        {
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

        // 🔄 Recuperación
        public async Task SendRecoveryCodeEmail(string emailReceptor, string recoveryCode)
        {
            EnsureValidEmail(emailReceptor, nameof(emailReceptor));

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

        // 🔐 Contraseña temporal
        public async Task SendTemporaryPasswordAsync(string email, string fullName, string tempPassword)
        {
            EnsureValidEmail(email, nameof(email));

            var content = $@"
                    <p>Hola <strong>{(fullName ?? "usuario")}</strong>, tu cuenta en <strong>{BrandName}</strong> ha sido creada exitosamente.</p>
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
