using Business.Interfaces;
using Data.Interfaz.IDataImplemenent.SecurityAuthentication;
using Data.Interfaz.Security;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Utilities.Helpers.Token;

namespace Business.CustomJWT
{
    /// <summary>
    /// Emisor y rotador de tokens JWT con refresh tokens persistidos.
    /// Opción B:
    ///  - Refuerza el hashing de refresh tokens con HMAC-SHA512 + pepper (JwtSettings.Key)
    ///  - Limpia claims del access token (sin duplicados) y agrega 'iat'
    ///  - Mantiene la arquitectura y dependencias existentes
    /// </summary>
    public class TokenBusiness : IToken
    {
        private readonly IUserRepository _userRepository;
        private readonly IRolUserRepository _rolUserRepository;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly JwtSettings _jwtSettings;
        private readonly IPasswordHasher<User> _passwordHasher;

        public TokenBusiness(
            IUserRepository userRepo,
            IRolUserRepository rolRepo,
            IRefreshTokenRepository refreshRepo,
            IOptions<JwtSettings> jwtSettings,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepo;
            _rolUserRepository = rolRepo;
            _refreshRepo = refreshRepo;
            _jwtSettings = jwtSettings.Value;
            _passwordHasher = passwordHasher;

            EnsureSigningKeyStrength(_jwtSettings.Key);
        }

        /// <summary>
        /// Login: valida credenciales y emite access token + refresh token (rotables) + CSRF.
        /// </summary>
        public async Task<(string AccessToken, string RefreshToken, string CsrfToken)> GenerateTokensAsync(LoginDto dto)
        {
            // 1) Validar credenciales
            var user = await _userRepository.GetByEmailProjectionAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("Usuario o contraseña inválida.");

            var pwdResult = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);
            if (pwdResult == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Usuario o contraseña inválida.");
            // if (pwdResult == PasswordVerificationResult.SuccessRehashNeeded) { /* rehash opcional */ }

            // 2) Generar access token con roles
            var roles = await _rolUserRepository.GetRoleNamesByUserIdAsync(user.Id);
            var accessToken = BuildAccessToken(user, roles);

            // 3) Generar refresh token (plain) y persistir su hash HMAC-SHA512 con pepper
            var now = DateTime.UtcNow;
            var refreshPlain = TokenHelpers.GenerateSecureRandomUrlToken(64);
            var refreshHash = HashRefreshToken(refreshPlain); // <- mejora de seguridad

            var refreshEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshHash,
                CreatedAt = now,
                ExpiresAt = now.AddDays(_jwtSettings.RefreshTokenExpirationDays)
            };
            await _refreshRepo.AddAsync(refreshEntity);

            // 3.1) Poda de tokens válidos por usuario (mantener tope de N)
            var validTokens = (await _refreshRepo.GetValidTokensByUserAsync(user.Id))
                              .OrderByDescending(t => t.CreatedAt)
                              .ToList();

            const int maxActiveRefreshTokens = 5; // ajusta según política
            if (validTokens.Count > maxActiveRefreshTokens)
            {
                foreach (var t in validTokens.Skip(maxActiveRefreshTokens))
                    await _refreshRepo.RevokeAsync(t);
            }

            // 4) CSRF token (client-side / cookie 'double-submit' pattern)
            var csrf = TokenHelpers.GenerateSecureRandomUrlToken(32);

            return (accessToken, refreshPlain, csrf);
        }

        /// <summary>
        /// Intercambia un refresh token válido por un nuevo access token y un nuevo refresh token (rotación).
        /// Aplica detección de reutilización (token revocado).
        /// </summary>
        public async Task<(string NewAccessToken, string NewRefreshToken)> RefreshAsync(string refreshTokenPlain, string remoteIp = null)
        {
            // Buscar por hash HMAC-SHA512 con pepper
            var hash = HashRefreshToken(refreshTokenPlain);
            var record = await _refreshRepo.GetByHashAsync(hash)
                ?? throw new SecurityTokenException("Refresh token inválido");

            if (record.ExpiresAt <= DateTime.UtcNow)
                throw new SecurityTokenException("Refresh token expirado");

            if (record.IsRevoked)
            {
                // Reutilización: revocar todos los tokens válidos del usuario
                var validTokens = await _refreshRepo.GetValidTokensByUserAsync(record.UserId);
                foreach (var t in validTokens)
                    await _refreshRepo.RevokeAsync(t);

                throw new SecurityTokenException("Refresh token inválido o reutilizado");
            }

            // Obtener usuario y roles
            var user = await _userRepository.GetByIdAsync(record.UserId)
                ?? throw new SecurityTokenException("Usuario no encontrado");

            var roles = await _rolUserRepository.GetRoleNamesByUserIdAsync(user.Id);

            // 1) Nuevo access token
            var newAccessToken = BuildAccessToken(user, roles);

            // 2) Rotación: crear nuevo refresh, persistir y revocar el anterior
            var now2 = DateTime.UtcNow;
            var newRefreshPlain = TokenHelpers.GenerateSecureRandomUrlToken(64);
            var newRefreshHash = HashRefreshToken(newRefreshPlain);

            var newRefreshEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = newRefreshHash,
                CreatedAt = now2,
                ExpiresAt = now2.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                // RemoteIpAddress = remoteIp // si tu entidad lo soporta
            };

            // Idealmente atómico si tu repositorio lo permite (BEGIN/COMMIT)
            await _refreshRepo.AddAsync(newRefreshEntity);
            await _refreshRepo.RevokeAsync(record, replacedByTokenHash: newRefreshHash);

            return (newAccessToken, newRefreshPlain);
        }

        /// <summary>
        /// Revoca explícitamente un refresh token (por su valor plano).
        /// </summary>
        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var hash = HashRefreshToken(refreshToken);
            var record = await _refreshRepo.GetByHashAsync(hash);
            if (record != null && !record.IsRevoked)
                await _refreshRepo.RevokeAsync(record);
        }

        /// <summary>
        /// Construye un access token JWT con claims mínimos y roles.
        /// - sub: user.Id
        /// - email: user.Email
        /// - jti: GUID por token
        /// - iat: epoch seconds (Integer64)
        /// - role: múltiples (filtrados y únicos)
        /// </summary>
        private string BuildAccessToken(User user, IEnumerable<string> roles)
        {
            var now = DateTime.UtcNow;
            var accessExp = now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                          new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                          ClaimValueTypes.Integer64)
            };

            foreach (var r in roles.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct())
                claims.Add(new Claim(ClaimTypes.Role, r));

            var jwt = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                notBefore: now,
                expires: accessExp,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        /// <summary>
        /// Hash de refresh tokens con HMAC-SHA512 usando como pepper la key del JWT.
        /// - No requiere columnas extra ni librerías nuevas.
        /// - Si cambias la key, los hashes antiguos no validarán (planifica rotación).
        /// </summary>
        private string HashRefreshToken(string token)
        {
            var pepper = Encoding.UTF8.GetBytes(_jwtSettings.Key);
            using var hmac = new HMACSHA512(pepper);
            var bytes = Encoding.UTF8.GetBytes(token);
            var mac = hmac.ComputeHash(bytes);
            return Convert.ToHexString(mac).ToLowerInvariant();
        }

        /// <summary>
        /// Verificación mínima de entropía: exige 32+ caracteres (≈256 bits) para HMAC-SHA256/512.
        /// </summary>
        private static void EnsureSigningKeyStrength(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || Encoding.UTF8.GetByteCount(key) < 32)
                throw new InvalidOperationException("JwtSettings.Key debe tener al menos 32 caracteres aleatorios (≥256 bits).");
        }
    }
}
