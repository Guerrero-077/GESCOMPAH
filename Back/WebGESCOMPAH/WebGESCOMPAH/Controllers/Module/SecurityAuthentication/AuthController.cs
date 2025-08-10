using Business.Interfaces;
using Business.Interfaces.Implements.SecrutityAuthentication;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Entity.DTOs.Implements.SecurityAuthentication.Auth.RestPasword;
using Entity.DTOs.Implements.SecurityAuthentication.Me;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
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
        private readonly IConfiguration _configuration;
        private readonly IAuthCookieFactory _cookieFactory;
        private readonly JwtSettings _jwt;
        private readonly CookieSettings _cookieSettings;

        public AuthController(IAuthService authService,
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
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Registrarse([FromBody] RegisterDto objeto)
        {
            await _authService.RegisterAsync(objeto);
            return Ok(new { isSuccess = true });
        }

        /// <summary>Login: genera access + refresh + csrf, guarda cookies HttpOnly.</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var (access, refresh, csrf, userContext) = await _tokenService.GenerateTokensAsync(dto);

            var now = DateTime.UtcNow;
            Response.Cookies.Append(_cookieSettings.AccessTokenName, access,
                _cookieFactory.AccessCookieOptions(now.AddMinutes(_jwt.AccessTokenExpirationMinutes)));
            Response.Cookies.Append(_cookieSettings.RefreshTokenName, refresh,
                _cookieFactory.RefreshCookieOptions(now.AddDays(_jwt.RefreshTokenExpirationDays)));
            Response.Cookies.Append(_cookieSettings.CsrfCookieName, csrf,
                _cookieFactory.CsrfCookieOptions(now.AddDays(_jwt.RefreshTokenExpirationDays)));

            return Ok(new
            {
                isSuccess = true,
                message = "Login exitoso",
                user = userContext
            });
        }

        /// <summary>Renueva tokens (usa refresh token cookie + comprobación CSRF double-submit).</summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshCookie = Request.Cookies[_cookieSettings.RefreshTokenName];
            if (string.IsNullOrWhiteSpace(refreshCookie))
                return Unauthorized();

            // Validar header CSRF
            if (!Request.Headers.TryGetValue("X-XSRF-TOKEN", out var headerValue))
                return Forbid();

            var csrfCookie = Request.Cookies[_cookieSettings.CsrfCookieName];
            if (string.IsNullOrWhiteSpace(csrfCookie) || csrfCookie != headerValue)
                return Forbid();

            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var (newAccess, newRefresh) = await _tokenService.RefreshAsync(refreshCookie, remoteIp);

            var now = DateTime.UtcNow;
            // Borrar antiguas cookies (usamos mismas opciones para garantizar eliminación)
            Response.Cookies.Delete(_cookieSettings.AccessTokenName, _cookieFactory.AccessCookieOptions(now));
            Response.Cookies.Delete(_cookieSettings.RefreshTokenName, _cookieFactory.RefreshCookieOptions(now));

            Response.Cookies.Append(_cookieSettings.AccessTokenName, newAccess,
                _cookieFactory.AccessCookieOptions(now.AddMinutes(_jwt.AccessTokenExpirationMinutes)));
            Response.Cookies.Append(_cookieSettings.RefreshTokenName, newRefresh,
                _cookieFactory.RefreshCookieOptions(now.AddDays(_jwt.RefreshTokenExpirationDays)));

            return Ok(new { isSuccess = true });
        }

        /// <summary>Logout: revoca refresh token y borra cookies.</summary>
        [HttpPost("logout")]
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



        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Token inválido o expirado.");

            var currentUserDto = await _authService.BuildUserContextAsync(userId);
            return Ok(currentUserDto);
        }

        [HttpPost("revoke-token")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RevokeToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new { message = "No refresh token provided" });

            await _tokenService.RevokeRefreshTokenAsync(refreshToken);

            Response.Cookies.Delete("refresh_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return Ok(new { message = "Refresh token revoked" });
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
