using Microsoft.Extensions.Logging;
using Utilities.Messaging.Interfaces;

namespace Utilities.Messaging.Implements
{
    /// <summary>
    /// Implementación que sólo registra en logs. Útil en desarrollo o testing.
    /// </summary>
    public class ConsoleEmailService : ISendCode
    {
        private readonly ILogger<ConsoleEmailService> _logger;

        public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendRecoveryCodeEmail(string emailReceptor, string recoveryCode)
        {
            _logger.LogInformation("[ConsoleEmail] RecoveryCode -> {Email}: {Code}", emailReceptor, recoveryCode);
            return Task.CompletedTask;
        }

        public Task SendTemporaryPasswordAsync(string email, string fullName, string tempPassword)
        {
            _logger.LogInformation("[ConsoleEmail] TempPassword -> {Email} ({Name}): {Password}", email, fullName, tempPassword);
            return Task.CompletedTask;
        }

        public Task SendContractWithPdfAsync(string email, string fullName, string contractNumber, byte[] pdfBytes)
        {
            _logger.LogInformation("[ConsoleEmail] ContractPDF -> {Email} ({Name}): Contrato #{ContractNumber}, PDF Size: {PdfSize} bytes", 
                email, fullName, contractNumber, pdfBytes?.Length ?? 0);
            return Task.CompletedTask;
        }
    }
}

