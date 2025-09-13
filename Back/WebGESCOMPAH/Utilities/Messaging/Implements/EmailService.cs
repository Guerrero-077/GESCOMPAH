using Microsoft.Extensions.Logging;
using Utilities.Messaging.Factories;
using Utilities.Messaging.Interfaces;

namespace Utilities.Messaging.Implements
{
    /// <summary>
    /// Fachada que delega en un proveedor elegido por fábrica.
    /// Mantiene la misma interfaz para no romper dependencias.
    /// </summary>
    public class EmailService : ISendCode
    {
        private readonly ISendCode _inner;
        private readonly ILogger<EmailService>? _logger;

        public EmailService(IEmailServiceFactory factory, ILogger<EmailService>? logger = null)
        {
            // Si no se especifica proveedor, la fábrica toma el de configuración
            _inner = factory.Create();
            _logger = logger;
            _logger?.LogDebug("EmailService inicializado con proveedor {Provider}", _inner.GetType().Name);
        }

        public Task SendRecoveryCodeEmail(string emailReceptor, string recoveryCode)
        {
            return _inner.SendRecoveryCodeEmail(emailReceptor, recoveryCode);
        }

        public Task SendTemporaryPasswordAsync(string email, string fullName, string tempPassword)
        {
            return _inner.SendTemporaryPasswordAsync(email, fullName, tempPassword);
        }

        public Task SendContractWithPdfAsync(string email, string fullName, string contractNumber, byte[] pdfBytes)
        {
            return _inner.SendContractWithPdfAsync(email, fullName, contractNumber, pdfBytes);
        }
    }
}
