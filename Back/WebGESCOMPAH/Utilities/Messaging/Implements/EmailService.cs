using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using Utilities.Messaging.Interfaces;

namespace Utilities.Messaging.Implements
{
    public class EmailService : ISendCode
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public async Task SendRecoveryCodeEmail(string emailReceptor, string recoveryCode)
        {
            var emailEmisor = _config["CONFIGURACIONES_EMAIL:EMAIL"]!;
            var password = _config["CONFIGURACIONES_EMAIL:PASSWORD"];
            var host = _config["CONFIGURACIONES_EMAIL:HOST"];
            var puerto = int.Parse(_config["CONFIGURACIONES_EMAIL:PUERTO"]!);

            var smtpCliente = new SmtpClient(host, puerto)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailEmisor, password)
            };

            string asunto = "Recuperación de contraseña";
            string cuerpoHtml = $@"
                <!DOCTYPE html>
                <html lang='es'>
                <head><meta charset='UTF-8'><title>Recuperación de Contraseña</title></head>
                <body style='font-family: Arial, sans-serif; background: #f4f4f4; padding: 40px;'>
                    <div style='max-width: 600px; margin: auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 5px 15px rgba(0,0,0,0.1);'>
                        <h2 style='color: #4a5568;'>Recupera tu contraseña</h2>
                        <p style='font-size: 16px; color: #2d3748;'>Hemos recibido una solicitud para restablecer tu contraseña.</p>
                        <p style='font-size: 16px; color: #2d3748;'>Tu código de verificación es:</p>
                        <div style='font-size: 28px; font-weight: bold; color: #667eea; margin: 20px 0;'>{recoveryCode}</div>
                        <p style='font-size: 14px; color: #718096;'>Este código tiene una validez de 10 minutos. Si no solicitaste este cambio, puedes ignorar este correo.</p>
                        <br>
                        <p style='font-size: 12px; color: #a0aec0;'>Portal Agro-Comercial del Huila © 2025</p>
                    </div>
                </body>
                </html>";

            var mensaje = new MailMessage(emailEmisor, emailReceptor, asunto, cuerpoHtml)
            {
                IsBodyHtml = true
            };

            await smtpCliente.SendMailAsync(mensaje);
        }
    }
}