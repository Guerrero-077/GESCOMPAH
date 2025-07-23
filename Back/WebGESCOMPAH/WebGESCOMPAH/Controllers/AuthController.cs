//using Business.Interfaces;
//using Business.Interfaces.Implements;
//using Common.Custom;
//using Data.Interfaz.IDataImplemenent;
//using Entity.DTOs.Implements.SecurityAuthentication;
//using Entity.DTOs.Implements.SecurityAuthentication.Auth;
//using Entity.DTOs.Implements.SecurityAuthentication.Auth.RestPasword;
//using MapsterMapper;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;
//using Utilities.Exceptions;

//namespace WebGESCOMPAH.Controllers
//{

//    [ApiController]
//    [Route("api/v1/[controller]")]
//    public class AuthController : ControllerBase
//    {
//        //private readonly IToken _token;
//        private readonly ILogger<AuthController> _logger;
//        private readonly IAuthService _authService;
//        private readonly IToken _token;
//        private readonly IDepartmentService _departmentService;
//        private readonly ICityService _cityService;
//        private readonly IConfiguration _configuration;
//        private readonly IUserRepository _userData;
//        private readonly IMapper _mapper;


//        public AuthController(
//            ILogger<AuthController> logger,
//            IAuthService authService,
//            IToken token,
//            IDepartmentService departmentService,
//            ICityService cityService,
//            IConfiguration configuration,
//            IUserRepository userData,
//            IMapper mapper)
//        {

//            _logger = logger;
//            _authService = authService;
//            _token = token;
//            _departmentService = departmentService;
//            _cityService = cityService;
//            _configuration = configuration;
//            _userData = userData;
//            _mapper = mapper;


//        }

//        [HttpPost]
//        [Route("Register")]
//        [ProducesResponseType(typeof(string), 200)]
//        [ProducesResponseType(400)]
//        [ProducesResponseType(500)]
//        public async Task<IActionResult> Registrarse(RegisterDto objeto)
//        {
//            try
//            {
//                var userCreated = await _authService.RegisterAsync(objeto);

//                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status400BadRequest, new { isSuccess = false, message = ex.Message });
//            }
//        }


//        [HttpPost("login")]
//        [ProducesResponseType(200)]
//        [ProducesResponseType(400)]
//        [ProducesResponseType(401)]
//        [ProducesResponseType(500)]
//        public async Task<IActionResult> Login([FromBody] LoginDto login)
//        {
//            try
//            {
//                var token = await _token.GenerateToken(login);

//                var cookieOptions = new CookieOptions
//                {
//                    HttpOnly = true,
//                    Secure = true, // ⚠️ en producción pon true
//                    SameSite = SameSiteMode.None,
//                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:exp"]))

//                };

//                Response.Cookies.Append("access_token", token, cookieOptions);

//                return Ok(new { isSuccess = true, message = "Login exitoso" });
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                _logger.LogWarning(ex, "Intento de inicio de sesión con credenciales inválidas");
//                return Unauthorized(new { message = ex.Message });
//            }
//            catch (ValidationException ex)
//            {
//                _logger.LogWarning(ex, "Validación fallida para el inicio de sesión");
//                return BadRequest(new { message = ex.Message });
//            }
//            catch (ExternalServiceException ex)
//            {
//                _logger.LogError(ex, "Error al crear el token");
//                return StatusCode(500, new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error inesperado en el login");
//                return StatusCode(500, new { message = "Error interno del servidor" });
//            }
//        }

//        [Authorize]
//        [HttpGet("me")]
//        public async Task<IActionResult> GetCurrentUser()
//        {
//            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

//            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
//                return Unauthorized("El token no contiene un Claim 'sub' (NameIdentifier) válido o no es un GUID.");

//            var currentUserDto = await _authService.BuildUserContextAsync(userId);

//            return Ok(currentUserDto);
//        }




//        //[HttpGet]
//        //[Route("ValidarToken")]
//        //public IActionResult ValidarToken([FromQuery] string token)

//        //{

//        //    bool respuesta = _token.validarToken(token);
//        //    return StatusCode(StatusCodes.Status200OK, new { isSuccess = respuesta });

//        //}

//        [HttpPost("recuperar/enviar-codigo")]
//        [ProducesResponseType(typeof(string), 200)]
//        [ProducesResponseType(400)]
//        [ProducesResponseType(401)]
//        [ProducesResponseType(500)]
//        public async Task<IActionResult> EnviarCodigoAsync([FromBody] RequestResetDto dto)
//        {
//            try
//            {
//                await _authService.RequestPasswordResetAsync(dto.Email);
//                return Ok(new { isSuccess = true, message = "Código enviado al correo (si el email es válido)" });
//            }
//            catch (ValidationException ex)
//            {
//                _logger.LogWarning(ex, "Validación fallida en solicitud de código de recuperación");
//                return BadRequest(new { message = ex.Message });
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                _logger.LogWarning(ex, "Correo no encontrado para recuperación");
//                return NotFound(new { message = ex.Message });
//            }
//            catch (ExternalServiceException ex)
//            {
//                _logger.LogError(ex, "Fallo al enviar el correo de recuperación");
//                return StatusCode(500, new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error inesperado al enviar código de recuperación");
//                return StatusCode(500, new { message = "Error interno del servidor" });
//            }
//        }

//        [HttpPost("recuperar/confirmar")]
//        [ProducesResponseType(typeof(string), 200)]
//        [ProducesResponseType(400)]
//        [ProducesResponseType(401)]
//        [ProducesResponseType(500)]
//        public async Task<IActionResult> ConfirmarCodigo([FromBody] ConfirmResetDto dto)
//        {
//            try
//            {
//                await _authService.ResetPasswordAsync(dto);
//                return Ok(new { isSuccess = true, message = "Contraseña actualizada" });
//            }
//            catch (ValidationException ex)
//            {
//                _logger.LogWarning(ex, "Validación fallida al confirmar código");
//                return BadRequest(new { message = ex.Message });
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                _logger.LogWarning(ex, "Código o usuario no encontrado");
//                return NotFound(new { message = ex.Message });
//            }
//            catch (ExternalServiceException ex)
//            {
//                _logger.LogError(ex, "Fallo al actualizar la contraseña");
//                return StatusCode(500, new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error inesperado al confirmar código de recuperación");
//                return StatusCode(500, new { message = "Error interno del servidor" });
//            }
//        }


//        [HttpGet("Department")]
//        [ProducesResponseType(200)]
//        [ProducesResponseType(500)]
//        public virtual async Task<IActionResult> Get()
//        {
//            try
//            {
//                var result = await _departmentService.GetAllAsync();
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error obteniendo datos");
//                return StatusCode(500, new { message = "Error interno del servidor." });
//            }

//            //var result = await DeleteAsync(id, deleteType);

//            //if (!result)
//            //    return NotFound(new { message = "No se pudo eliminar el recurso." });

//            //return Ok(new { message = $"Eliminación {deleteType} realizada correctamente." });
//        }


//        [HttpGet("City/{id}")]
//        //[ProducesResponseType(typeof(TDto), 200)]
//        [ProducesResponseType(200)]
//        [ProducesResponseType(400)]
//        [ProducesResponseType(404)]
//        [ProducesResponseType(500)]
//        public virtual async Task<IActionResult> GetById(int id)
//        {
//            try
//            {
//                var result = await _cityService.GetCityByDepartment(id);
//                if (result == null)
//                    return NotFound(new { message = $"No se encontró el elemento con ID {id}" });

//                return Ok(result);
//            }
//            catch (ValidationException ex)
//            {
//                _logger.LogWarning(ex, "Validación fallida con ID: {Id}", id);
//                return BadRequest(new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error al obtener el ID {Id}", id);
//                return StatusCode(500, new { message = "Error interno del servidor." });
//            }
//        }

//    }
//}