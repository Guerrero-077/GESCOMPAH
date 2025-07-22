using Business.Interfaces;
using Common.Custom;
using Data.Interfaz.IDataImplemenent;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;
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
            private readonly IUserRepository _userData;
            private readonly IRolUserRepository _rolUserData;
            public TokenBusiness(IConfiguration configuration, IUserRepository userData, IRolUserRepository rolUserData)
            {
                _configuration = configuration;
                _userData = userData;
                _rolUserData = rolUserData;

            }
        public async Task<string> GenerateToken(LoginDto dto)
        {
            dto.Password = EncriptePassword.EncripteSHA256(dto.Password);
            var user = await _userData.LoginUser(dto);

            if (user == null)
                throw new UnauthorizedAccessException("Usuario o contraseña inválida.");

            var roles = await _rolUserData.GetRolesByUserIdAsync(user.Id);

            var userClaims = new List<Claim>
    {
        new Claim("Id", user.Id.ToString()), // opcional
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // estándar
        new Claim(ClaimTypes.Email, dto.Email!)
    };

            userClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

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
