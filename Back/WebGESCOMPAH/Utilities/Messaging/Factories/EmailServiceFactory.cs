using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Utilities.Messaging.Implements;
using Utilities.Messaging.Interfaces;

namespace Utilities.Messaging.Factories
{
    public class EmailServiceFactory : IEmailServiceFactory
    {
        private readonly IConfiguration _config;
        private readonly ILoggerFactory _loggerFactory;

        public EmailServiceFactory(IConfiguration config, ILoggerFactory loggerFactory)
        {
            _config = config;
            _loggerFactory = loggerFactory;
        }

        public ISendCode Create(string? providerName = null)
        {
            var provider = providerName
                ?? _config["CONFIGURACIONES_EMAIL:PROVIDER"]
                ?? "smtp"; // valor por defecto

            switch (provider.Trim().ToLowerInvariant())
            {
                case "smtp":
                    return new SmtpEmailService(
                        _config,
                        _loggerFactory.CreateLogger<SmtpEmailService>());

                case "console":
                case "log":
                case "noop":
                    return new ConsoleEmailService(
                        _loggerFactory.CreateLogger<ConsoleEmailService>());

                default:
                    // Desconocido -> por seguridad usa console/log para no fallar en runtime silenciosamente
                    var logger = _loggerFactory.CreateLogger<EmailServiceFactory>();
                    logger.LogWarning("Proveedor de email desconocido '{Provider}'. Usando 'console'", provider);
                    return new ConsoleEmailService(
                        _loggerFactory.CreateLogger<ConsoleEmailService>());
            }
        }
    }
}

