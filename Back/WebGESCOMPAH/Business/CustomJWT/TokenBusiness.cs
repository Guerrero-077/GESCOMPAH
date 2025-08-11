using Business.Interfaces;
using Business.Interfaces.Implements.SecrutityAuthentication;
using Data.Interfaz.IDataImplemenent.SecurityAuthentication;
using Data.Interfaz.Security;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Entity.DTOs.Implements.SecurityAuthentication.Me;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Utilities.Helpers.Token;

namespace Business.CustomJWT
{
    public class TokenBusiness : IToken
    {
        private readonly IUserRepository _userRepository;
        private readonly IRolUserRepository _rolUserRepository;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly JwtSettings _jwtSettings;
        private readonly IAuthService _authService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public TokenBusiness(
            IUserRepository userRepo,
            IRolUserRepository rolRepo,
            IRefreshTokenRepository refreshRepo,
            IOptions<JwtSettings> jwtSettings,
            IAuthService authService,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepo;
            _rolUserRepository = rolRepo;
            _refreshRepo = refreshRepo;
            _jwtSettings = jwtSettings.Value;
            _authService = authService;
            _passwordHasher = passwordHasher;
        }

        public async Task<(string AccessToken, string RefreshToken, string CsrfToken, UserMeDto user)> GenerateTokensAsync(LoginDto dto)
        {
            // 1. Validar credenciales
            var user = await _userRepository.GetByEmailProjectionAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("Usuario o contraseña inválida.");

            if (_passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password) == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Usuario o contraseña inválida.");

            // 2. Generar tokens (con roles)
            var roles = await _rolUserRepository.GetRoleNamesByUserIdAsync(user.Id);
            var accessToken = BuildAccessToken(user, roles);

            // 3. Refresh Token (hash para persistencia)
            var now = DateTime.UtcNow;
            var refreshPlain = TokenHelpers.GenerateSecureRandomUrlToken(64);
            var refreshHash = TokenHelpers.ComputeSha256Hex(refreshPlain);

            var refreshEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshHash,
                CreatedAt = now,
                ExpiresAt = now.AddDays(_jwtSettings.RefreshTokenExpirationDays)
            };
            await _refreshRepo.AddAsync(refreshEntity);

            // Opcional: podar tokens antiguos manteniendo un máximo de N tokens válidos por usuario
            var validTokens = (await _refreshRepo.GetValidTokensByUserAsync(user.Id))
                                .OrderByDescending(t => t.CreatedAt)
                                .ToList();
            const int maxActiveRefreshTokens = 5; // ajustar según política
            if (validTokens.Count > maxActiveRefreshTokens)
            {
                foreach (var t in validTokens.Skip(maxActiveRefreshTokens))
                {
                    await _refreshRepo.RevokeAsync(t);
                }
            }

            // 4. CSRF token
            var csrf = TokenHelpers.GenerateSecureRandomUrlToken(32);

            // 5. Construir contexto de usuario completo
            var userContext = await _authService.BuildUserContextAsync(user.Id);

            return (accessToken, refreshPlain, csrf, userContext);
        }





        public async Task<(string NewAccessToken, string NewRefreshToken)> RefreshAsync(string refreshTokenPlain, string remoteIp = null)
        {
            var hash = TokenHelpers.ComputeSha256Hex(refreshTokenPlain);
            var record = await _refreshRepo.GetByHashAsync(hash)
                ?? throw new SecurityTokenException("Refresh token inválido");

            if (record.ExpiresAt <= DateTime.UtcNow)
                throw new SecurityTokenException("Refresh token expirado");

            if (record.IsRevoked)
            {
                // Reutilización detectada: revocar todos los tokens válidos del usuario
                var validTokens = await _refreshRepo.GetValidTokensByUserAsync(record.UserId);
                foreach (var t in validTokens)
                {
                    await _refreshRepo.RevokeAsync(t);
                }
                throw new SecurityTokenException("Refresh token inválido o reutilizado");
            }

            // Obtener usuario y roles
            var user = await _userRepository.GetByIdAsync(record.UserId)
                ?? throw new SecurityTokenException("Usuario no encontrado");

            var roles = await _rolUserRepository.GetRoleNamesByUserIdAsync(user.Id);

            // Nuevo access token
            var newAccessToken = BuildAccessToken(user, roles);

            // ROTATION: crear nuevo refresh token, persistir y revocar el anterior
            var newRefreshPlain = TokenHelpers.GenerateSecureRandomUrlToken(64);
            var newRefreshHash = TokenHelpers.ComputeSha256Hex(newRefreshPlain);

            var now2 = DateTime.UtcNow;
            var newRefreshEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = newRefreshHash,
                CreatedAt = now2,
                ExpiresAt = now2.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                //RemoteIpAddress = remoteIp
            };

            await _refreshRepo.AddAsync(newRefreshEntity);
            await _refreshRepo.RevokeAsync(record, replacedByTokenHash: newRefreshHash);

            return (newAccessToken, newRefreshPlain);
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var hash = TokenHelpers.ComputeSha256Hex(refreshToken);
            var record = await _refreshRepo.GetByHashAsync(hash);
            if (record != null && !record.IsRevoked)
            {
                await _refreshRepo.RevokeAsync(record);
            }
        }
    private string BuildAccessToken(User user, IEnumerable<string> roles)
        {
            var now = DateTime.UtcNow;
            var accessExp = now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

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
    }
}