using Business.Interfaces;
using Business.Interfaces.Implements.SecrutityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Entity.DTOs.Implements.SecurityAuthentication.Auth.RestPasword;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers.Implements.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        private readonly IToken _token;
        private readonly IConfiguration _configuration;

        public AuthController(ILogger<AuthController> logger,
            IAuthService authService, IToken token, IConfiguration configuration)
        {
            _logger = logger;
            _authService = authService;
            _token = token;
            _configuration = configuration;
        }

        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Registrarse([FromBody] RegisterDto objeto)
        {
            await _authService.RegisterAsync(objeto);
            return Ok(new { isSuccess = true });
        }

        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var token = await _token.GenerateToken(login);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // en producción debe estar en true
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:exp"]))
            };

            Response.Cookies.Append("access_token", token, cookieOptions);

            return Ok(new { isSuccess = true, token, message = "Login exitoso" });
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("El token no contiene un Claim 'sub' (NameIdentifier) válido o no es un ID.");

            var currentUserDto = await _authService.BuildUserContextAsync(userId);
            return Ok(currentUserDto);
        }

        [HttpPost("logout")]
        [ProducesResponseType(200)]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            return Ok(new { message = "Sesión cerrada" });
        }

        [HttpPost("recuperar/enviar-codigo")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> EnviarCodigoAsync([FromBody] RequestResetDto dto)
        {
            await _authService.RequestPasswordResetAsync(dto.Email);
            return Ok(new { isSuccess = true, message = "Código enviado al correo (si el email es válido)" });
        }

        [HttpPost("recuperar/confirmar")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ConfirmarCodigo([FromBody] ConfirmResetDto dto)
        {
            await _authService.ResetPasswordAsync(dto);
            return Ok(new { isSuccess = true, message = "Contraseña actualizada" });
        }
    }
}
