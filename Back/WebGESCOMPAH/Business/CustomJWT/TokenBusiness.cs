using Business.Interfaces;
using Data.Interfaz.IDataImplemenent.SecurityAuthentication;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.CustomJWT
{
    public class TokenBusiness : IToken
    {
        private readonly IUserRepository _userRepository;
        private readonly IRolUserRepository _rolUserRepository;
        private readonly IMemoryCache _cache;
        private readonly JwtSettings _jwtSettings;

        public TokenBusiness(
            IUserRepository userRepo,
            IRolUserRepository rolRepo,
            IMemoryCache cache,
            IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepo;
            _rolUserRepository = rolRepo;
            _cache = cache;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<string> GenerateToken(LoginDto dto)
        {
            // 1. Proyección ligera del usuario desde la base de datos
            var user = await _userRepository.GetByEmailProjectionAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("Usuario o contraseña inválida.");

            // 2. Verificación del hash de contraseña
            var hasher = new PasswordHasher<User>();
            if (hasher.VerifyHashedPassword(user, user.Password, dto.Password) == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Usuario o contraseña inválida.");

            // 3. Obtención cacheada de roles por usuario
            var roles = await _cache.GetOrCreateAsync($"UserRoles_{user.Id}", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return _rolUserRepository.GetRoleNamesByUserIdAsync(user.Id);
            });

            // 4. Construcción de claims
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            // 5. Creación de token JWT firmado con clave simétrica
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
