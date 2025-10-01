using Utilities.Messaging.Interfaces;

namespace Utilities.Messaging.Factories
{
    public interface IEmailServiceFactory
    {
        // Crea un proveedor de correo basado en nombre/config
        ISendCode Create(string? providerName = null);
    }
}

