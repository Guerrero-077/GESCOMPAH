using Business.CQRS.SecrutityAuthentication.Rol;
using Business.CQRS.SecrutityAuthentication.Rol.Create;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.AdministrationSystem
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class RolController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Crea una nueva persona
        /// </summary>
        [HttpPost("CrearRol")]
        public async Task<IActionResult> Create([FromBody] RolDto rolDto)
        {
            var command = new CreateRolCommand(rolDto);
            var newRolId = await _mediator.Send(command);

            // Retorna 201 Created sin CreatedAtAction ni GetById
            return StatusCode(StatusCodes.Status201Created, new { id = newRolId });
        }
    }
}
