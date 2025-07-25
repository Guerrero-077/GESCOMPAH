using Business.Interfaces.Implements.AdministrationSystem;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.AdministrationSystem
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class RolController : ControllerBase
    {
        private readonly IRolService _rolService;

        public RolController(IRolService rolService)
        {
            _rolService = rolService;
        }

        /// <summary>
        /// Crea una nueva persona
        /// </summary>
        [HttpPost("CrearRol")]
        public async Task<IActionResult> Create([FromBody] RolDto rolDto)
        {
            var result = await _rolService.CreateAsync(rolDto);
            return Ok(result);
        }
    }
}
