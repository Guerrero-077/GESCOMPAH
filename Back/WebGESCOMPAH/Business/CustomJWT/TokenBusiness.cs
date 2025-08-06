using Business.Interfaces;
using Data.Interfaz.IDataImplemenent.SecurityAuthentication;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.CustomJWT
    {
    public class TokenBusiness : IToken
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IRolUserRepository _rolUserRepository;

        public TokenBusiness(IConfiguration configuration, IUserRepository userRepository, IRolUserRepository rolUserRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _rolUserRepository = rolUserRepository;
        }

        public async Task<string> GenerateToken(LoginDto dto)
        {
            // 1. Buscar el usuario por email
            var user = await _userRepository.GetByEmailAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("Usuario o contraseña inválida.");

            // 2. Verificar la contraseña usando el hash
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, dto.Password);

            if (result == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Usuario o contraseña inválida.");

            // 3. Obtener roles del usuario
            var roles = await _rolUserRepository.GetRolesByUserIdAsync(user.Id);

            // 4. Generar claims
            var userClaims = new List<Claim>
        {
            new Claim("Id", user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

            userClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

            // 5. Crear token JWT
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: userClaims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:exp"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
