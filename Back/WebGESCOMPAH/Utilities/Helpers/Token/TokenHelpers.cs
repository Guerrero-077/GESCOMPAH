using System.Security.Cryptography;
using System.Text;

namespace Utilities.Helpers.Token
{
    public static class TokenHelpers
    {
        /// <summary>Genera un string aleatorio seguro en base64 (puede contener + / =).</summary>
        public static string GenerateSecureRandomString(int size = 64)
        {
            var bytes = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Genera un string aleatorio seguro en Base64URL (sin + / =) adecuado para URLs, headers y cookies.
        /// </summary>
        public static string GenerateSecureRandomUrlToken(int size = 64)
        {
            var bytes = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            var base64 = Convert.ToBase64String(bytes);
            return base64.Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        /// <summary>Calcula SHA256 y devuelve hex en mayúsculas (compatible con ComputeHex).</summary>
        public static string ComputeSha256Hex(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }
    }
}
