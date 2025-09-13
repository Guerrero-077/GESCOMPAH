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
                    if (!HasValidSmtpConfig())
                    {
                        var loggerSmtp = _loggerFactory.CreateLogger<EmailServiceFactory>();
                        loggerSmtp.LogWarning(
                            "CONFIGURACIONES_EMAIL incompleto o inválido para 'smtp'. Cayendo a 'console' (no envía correos reales). Configure EMAIL, PASSWORD, HOST y PUERTO.");
                        return new ConsoleEmailService(
                            _loggerFactory.CreateLogger<ConsoleEmailService>());
                    }
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

        private bool HasValidSmtpConfig()
        {
            try
            {
                var email = _config["CONFIGURACIONES_EMAIL:EMAIL"];
                var password = _config["CONFIGURACIONES_EMAIL:PASSWORD"];
                var host = _config["CONFIGURACIONES_EMAIL:HOST"];
                var portStr = _config["CONFIGURACIONES_EMAIL:PUERTO"];

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)  || string.IsNullOrWhiteSpace(host))
                    return false;

                if (!int.TryParse(portStr, out var port) || port <= 0)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

