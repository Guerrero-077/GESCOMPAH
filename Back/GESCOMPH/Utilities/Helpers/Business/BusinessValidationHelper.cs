using Entity.Domain.Models.ModelBase;

namespace Utilities.Helpers.Business
{
    public static class BusinessValidationHelper
    {
        /// <summary>
        /// Lanza una excepción si el objeto es nulo.
        /// </summary>
        /// <param name="obj">Objeto a validar</param>
        /// <param name="message">Mensaje de error para la excepción</param>
        /// <exception cref="InvalidOperationException">Se lanza cuando el objeto es nulo</exception>
        public static void ThrowIfNull(object? obj, string message)
        {
            if (obj == null)
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Lanza una excepción si la condición es verdadera.
        /// </summary>
        /// <param name="condition">Condición a evaluar</param>
        /// <param name="message">Mensaje de error para la excepción</param>
        /// <exception cref="InvalidOperationException">Se lanza cuando la condición es verdadera</exception>
        public static void ThrowIfTrue(bool condition, string message)
        {
            if (condition)
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Lanza una excepción si la cadena es nula, vacía o solo contiene espacios en blanco.
        /// </summary>
        /// <param name="value">Cadena a validar</param>
        /// <param name="message">Mensaje de error para la excepción</param>
        /// <exception cref="InvalidOperationException">Se lanza cuando la cadena es nula o vacía</exception>
        public static void ThrowIfNullOrEmpty(string? value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Lanza una excepción si el número es negativo.
        /// </summary>
        /// <param name="number">Número a validar</param>
        /// <param name="message">Mensaje de error para la excepción</param>
        /// <exception cref="InvalidOperationException">Se lanza cuando el número es negativo</exception>
        public static void ThrowIfNegative(int number, string message)
        {
            if (number < 0)
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Lanza una excepción si el número es cero o menor.
        /// </summary>
        /// <param name="number">Número a validar</param>
        /// <param name="message">Mensaje de error para la excepción</param>
        /// <exception cref="InvalidOperationException">Se lanza cuando el número es ≤ 0</exception>
        public static void ThrowIfZeroOrLess(int number, string message)
        {
            if (number <= 0)
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Lanza una excepción si la entidad está marcada como eliminada lógicamente.
        /// </summary>
        /// <param name="entity">Entidad que implementa ISupportLogicalDelete</param>
        /// <param name="message">Mensaje de error para la excepción</param>
        /// <exception cref="InvalidOperationException">Se lanza cuando.IsDeleted es true</exception>
        public static void ThrowIfDeleted(BaseModel entity, string message)
        {
            if (entity.IsDeleted)
                throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Lanza una excepción si la entidad NO está marcada como eliminada lógicamente.
        /// </summary>
        /// <param name="entity">Entidad que implementa ISupportLogicalDelete</param>
        /// <param name="message">Mensaje de error para la excepción</param>
        /// <exception cref="InvalidOperationException">Se lanza cuando.IsDeleted es false</exception>
        public static void ThrowIfNotDeleted(BaseModel entity, string message)
        {
            if (!entity.IsDeleted)
                throw new InvalidOperationException(message);
        }
    }
}