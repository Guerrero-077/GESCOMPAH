    using Business.Interfaces;
    using Data.Interfaz.IDataImplemenent;
    using Entity.DTOs.Implements.SecurityAuthentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Utilities.Custom;

    namespace Business.CustomJWT
    {
        public class TokenBusiness : IToken
        {

            private readonly IConfiguration _configuration;
            private readonly IUserRepository _userData;
            private readonly IRolUserRepository _rolUserData;
            private readonly EncriptePassword _utilities;
            public TokenBusiness(IConfiguration configuration, IUserRepository userData, IRolUserRepository rolUserData, EncriptePassword utilities)
            {
                _configuration = configuration;
                _userData = userData;
                _rolUserData = rolUserData;
                _utilities = utilities;

            }
        public async Task<string> GenerateToken(LoginDto dto)
        {
            dto.Password = _utilities.EncripteSHA256(dto.Password);
            var user = await _userData.LoginUser(dto);

            // 🚨 Validación adicional si no lo haces en IUserRepository.LoginUser
            if (user == null)
                throw new UnauthorizedAccessException("Usuario o contraseña inválida.");

            // 🟡 Obtener roles del usuario
            var roles = await _rolUserData.GetRolesByUserIdAsync(user.Id); // Este método lo defines tú

            // 🟢 Crear claims base
            var userClaims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, dto.Email!)
            };

            // 🟣 Agregar roles como múltiples claims "role"
            userClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

            // 🔐 Generar token
            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!));
            var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var jwtConfig = new JwtSecurityToken
            (
                claims: userClaims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:exp"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }



    }
}
