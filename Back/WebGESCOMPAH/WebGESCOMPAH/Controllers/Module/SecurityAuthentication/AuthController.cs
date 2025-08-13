using Business.Interfaces;
using Business.Interfaces.Implements.SecrutityAuthentication;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Entity.DTOs.Implements.SecurityAuthentication.Auth.RestPasword;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebGESCOMPAH.Infrastructure;

namespace WebGESCOMPAH.Controllers.Module.SecurityAuthentication
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        private readonly IToken _tokenService;
        private readonly IAuthCookieFactory _cookieFactory;
        private readonly JwtSettings _jwt;
        private readonly CookieSettings _cookieSettings;

        public AuthController(
            IAuthService authService,
            IToken tokenService,
            IAuthCookieFactory cookieFactory,
            IOptions<JwtSettings> jwtOptions,
            IOptions<CookieSettings> cookieOptions,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _tokenService = tokenService;
            _cookieFactory = cookieFactory;
            _jwt = jwtOptions.Value;
            _cookieSettings = cookieOptions.Value;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Registrarse([FromBody] RegisterDto objeto)
        {
            await _authService.RegisterAsync(objeto);
            return Ok(new { isSuccess = true });
        }

        /// <summary>Login: genera access + refresh + csrf, guarda cookies HttpOnly.</summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
        {
            // Ahora GenerateTokensAsync NO retorna userContext
            var (access, refresh, csrf) = await _tokenService.GenerateTokensAsync(dto);

            var now = DateTime.UtcNow;

            // Setear cookies usando tu fábrica (mismas opciones para escribir y borrar)
            Response.Cookies.Append(
                _cookieSettings.AccessTokenName,
                access,
                _cookieFactory.AccessCookieOptions(now.AddMinutes(_jwt.AccessTokenExpirationMinutes)));

            Response.Cookies.Append(
                _cookieSettings.RefreshTokenName,
                refresh,
                _cookieFactory.RefreshCookieOptions(now.AddDays(_jwt.RefreshTokenExpirationDays)));

            Response.Cookies.Append(
                _cookieSettings.CsrfCookieName,
                csrf,
                _cookieFactory.CsrfCookieOptions(now.AddDays(_jwt.RefreshTokenExpirationDays)));

            // Respuesta mínima (el /auth/me devolverá el contexto completo cuando el front lo pida)
            return Ok(new
            {
                isSuccess = true,
                message = "Login exitoso"
            });
        }


        /// <summary>Renueva tokens (usa refresh cookie + comprobación CSRF double-submit).</summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Refresh(CancellationToken ct)
        {
            var refreshCookie = Request.Cookies[_cookieSettings.RefreshTokenName];
            if (string.IsNullOrWhiteSpace(refreshCookie))
                return Unauthorized();

            // Validación CSRF (double-submit: header debe igualar cookie)
            if (!Request.Headers.TryGetValue("X-XSRF-TOKEN", out var headerValue))
                return Forbid();

            var csrfCookie = Request.Cookies[_cookieSettings.CsrfCookieName];
            if (string.IsNullOrWhiteSpace(csrfCookie) || csrfCookie != headerValue)
                return Forbid();

            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var (newAccess, newRefresh) = await _tokenService.RefreshAsync(refreshCookie, remoteIp);

            var now = DateTime.UtcNow;

            // Eliminar cookies anteriores con las MISMAS opciones (path/domain/samesite) para asegurar borrado
            Response.Cookies.Delete(_cookieSettings.AccessTokenName, _cookieFactory.AccessCookieOptions(now));
            Response.Cookies.Delete(_cookieSettings.RefreshTokenName, _cookieFactory.RefreshCookieOptions(now));

            // Escribir nuevas
            Response.Cookies.Append(
                _cookieSettings.AccessTokenName,
                newAccess,
                _cookieFactory.AccessCookieOptions(now.AddMinutes(_jwt.AccessTokenExpirationMinutes)));

            Response.Cookies.Append(
                _cookieSettings.RefreshTokenName,
                newRefresh,
                _cookieFactory.RefreshCookieOptions(now.AddDays(_jwt.RefreshTokenExpirationDays)));

            return Ok(new { isSuccess = true });
        }

        /// <summary>Logout: revoca refresh token y borra cookies.</summary>
        [HttpPost("logout")]
        [AllowAnonymous] // puede hacerse sin estar autenticado si solo borra cookies
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            var refreshCookie = Request.Cookies[_cookieSettings.RefreshTokenName];
            if (!string.IsNullOrWhiteSpace(refreshCookie))
            {
                await _tokenService.RevokeRefreshTokenAsync(refreshCookie);
            }

            var now = DateTime.UtcNow;
            Response.Cookies.Delete(_cookieSettings.AccessTokenName, _cookieFactory.AccessCookieOptions(now));
            Response.Cookies.Delete(_cookieSettings.RefreshTokenName, _cookieFactory.RefreshCookieOptions(now));
            Response.Cookies.Delete(_cookieSettings.CsrfCookieName, _cookieFactory.CsrfCookieOptions(now));

            return Ok(new { message = "Sesión cerrada" });
        }

        /// <summary>/me: retorna el contexto del usuario actual.</summary>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUser()
        {
            // Con la opción B del TokenBusiness, el identificador está en 'sub'
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                   ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // fallback por si migras gradualmente

            if (string.IsNullOrWhiteSpace(sub) || !int.TryParse(sub, out var userId))
                return Unauthorized("Token inválido o expirado.");

            var currentUserDto = await _authService.BuildUserContextAsync(userId);
            return Ok(currentUserDto);
        }

        /// <summary>Revoca el refresh token actual (si existe) y elimina la cookie.</summary>
        [HttpPost("revoke-token")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RevokeToken()
        {
            var refreshToken = Request.Cookies[_cookieSettings.RefreshTokenName];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest(new { message = "No hay refresh token" });

            await _tokenService.RevokeRefreshTokenAsync(refreshToken);

            var now = DateTime.UtcNow;
            Response.Cookies.Delete(_cookieSettings.RefreshTokenName, _cookieFactory.RefreshCookieOptions(now));

            return Ok(new { message = "Refresh token revocado" });
        }

        [HttpPost("recuperar/enviar-codigo")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EnviarCodigoAsync([FromBody] RequestResetDto dto)
        {
            await _authService.RequestPasswordResetAsync(dto.Email);
            return Ok(new { isSuccess = true, message = "Código enviado al correo (si el email es válido)" });
        }

        [HttpPost("recuperar/confirmar")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmarCodigo([FromBody] ConfirmResetDto dto)
        {
            await _authService.ResetPasswordAsync(dto);
            return Ok(new { isSuccess = true, message = "Contraseña actualizada" });
        }
    }
}
