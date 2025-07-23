using Business.CQRS.Auth.Commands.Login;
using Business.CQRS.Auth.Commands.Register;
using Business.CQRS.Auth.Query.Me;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public AuthController(
        IMediator mediator,
        IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    [HttpPost("register")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var userDto = await _mediator.Send(command);
        return Ok(new { isSuccess = true, user = userDto });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var token = await _mediator.Send(command);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // En producción debe ser true
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:exp"]))
        };

        Response.Cookies.Append("access_token", token, cookieOptions);

        return Ok(new { isSuccess = true, token, message = "Login exitoso" });
    }

    [Authorize]
    [HttpGet("Me")]
    public async Task<IActionResult> Get()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Token inválido o sin claim NameIdentifier");
        }

        var userContext = await _mediator.Send(new GetUserContextQuery(userId));
        return Ok(userContext);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(200)]
    public IActionResult Logout()
    {
        if (Request.Cookies.ContainsKey("access_token"))
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1) // Expira en pasado para eliminar
            };
            Response.Cookies.Append("access_token", "", cookieOptions);
        }

        return Ok(new { isSuccess = true, message = "Logout exitoso" });
    }
}
