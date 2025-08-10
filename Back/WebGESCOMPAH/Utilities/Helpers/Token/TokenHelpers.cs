using System.Security.Cryptography;
using System.Text;

namespace Utilities.Helpers.Token
{
    public static class TokenHelpers
    {
        /// <summary>Genera un string aleatorio seguro en base64.</summary>
        public static string GenerateSecureRandomString(int size = 64)
        {
            var bytes = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
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
