using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;
using Utilities.Messaging.Interfaces;

namespace Utilities.Messaging.Implements
{
    /// <summary>
    /// Implementación basada en SMTP real (antes: EmailService).
    /// </summary>
    public class SmtpEmailService : ISendCode
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailService>? _logger;

        private const string BrandName = "GESCOMPAH";
        private const string BrandPrimary = "#2E7D32";
        private const string BrandAccent = "#16a34a";
        private const string BrandText = "#1f2937";
        private const string BrandMuted = "#6b7280";
        private const string BrandBorder = "#e5e7eb";

        public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService>? logger = null)
        {
            _config = config;
            _logger = logger;
        }

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
            var enableSslStr = _config["CONFIGURACIONES_EMAIL:ENABLE_SSL"];

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

            return (emailEmisor!, password!, host!, puerto, enableSsl);
        }

        private async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            EnsureValidEmail(to, nameof(to));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("El asunto no puede ser nulo o vacío.", nameof(subject));
            if (string.IsNullOrWhiteSpace(htmlBody))
                throw new ArgumentException("El cuerpo del correo no puede ser nulo o vacío.", nameof(htmlBody));

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
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error general enviando correo a {To}", to);
                throw;
            }
        }

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
                    <div style='background:{BrandPrimary}; padding:16px 20px; border-radius:12px 12px 0 0;'>
                      {logoHtml}
                    </div>
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

        public async Task SendContractWithPdfAsync(string email, string fullName, string contractNumber, byte[] pdfBytes)
        {
            EnsureValidEmail(email, nameof(email));
            
            if (pdfBytes == null || pdfBytes.Length == 0)
                throw new ArgumentException("El PDF del contrato no puede ser nulo o vacío.", nameof(pdfBytes));

            var content = $@"
                    <p>Estimado/a <strong>{(fullName ?? "usuario")}</strong>,</p>
                    <p>Nos complace informarle que su <strong>contrato de arrendamiento #{contractNumber}</strong> ha sido generado exitosamente.</p>
                    <p>Adjunto a este correo encontrará el documento PDF con todos los términos y condiciones acordados.</p>
                    <div style='background:#f8f9fa; border-left:4px solid {BrandPrimary}; padding:16px; margin:16px 0; border-radius:6px;'>
                        <p style='margin:0; font-weight:600; color:{BrandText};'>📄 Contrato #{contractNumber}</p>
                        <p style='margin:4px 0 0 0; color:{BrandMuted}; font-size:14px;'>Documento adjunto en formato PDF</p>
                    </div>
                    <p>Le recomendamos revisar cuidadosamente el documento y conservar una copia para sus registros.</p>
                    <p>Si tiene alguna pregunta o requiere aclaraciones, no dude en contactarnos.</p>";

            var html = WrapEmail("Contrato de Arrendamiento Generado", content);
            await SendEmailWithAttachmentAsync(email, "GESCOMPAH – Contrato de Arrendamiento", html, pdfBytes, $"Contrato_{contractNumber}.pdf");
        }

        private async Task SendEmailWithAttachmentAsync(string to, string subject, string htmlBody, byte[] attachmentBytes, string attachmentName)
        {
            EnsureValidEmail(to, nameof(to));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("El asunto no puede ser nulo o vacío.", nameof(subject));
            if (string.IsNullOrWhiteSpace(htmlBody))
                throw new ArgumentException("El cuerpo del correo no puede ser nulo o vacío.", nameof(htmlBody));

            var (fromEmail, password, host, port, enableSsl) = LoadSmtpConfig();

            using var smtpCliente = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail, password)
            };

            var displayName = $"{BrandName} • Contratos";
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

            // Agregar el PDF como adjunto
            if (attachmentBytes != null && attachmentBytes.Length > 0)
            {
                var attachment = new Attachment(new MemoryStream(attachmentBytes), attachmentName, "application/pdf");
                mensaje.Attachments.Add(attachment);
            }

            try
            {
                await smtpCliente.SendMailAsync(mensaje);
                _logger?.LogInformation("Email con adjunto PDF enviado a {To} con asunto {Subject}", to, subject);
            }
            catch (SmtpException ex)
            {
                _logger?.LogError(ex, "SMTP error enviando correo con adjunto a {To} (host {Host}:{Port})", to, host, port);
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error general enviando correo con adjunto a {To}", to);
                throw;
            }
        }
    }
}

