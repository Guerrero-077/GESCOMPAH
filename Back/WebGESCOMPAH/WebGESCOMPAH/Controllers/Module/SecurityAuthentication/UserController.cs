using Business.Interfaces.IBusiness;
using Business.Interfaces.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.SecurityAuthentication
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public sealed class UserController
        : BaseController<UserSelectDto, UserCreateDto, UserUpdateDto>
    {
        private readonly IUserService _userService;

        public UserController(IUserService service, ILogger<UserController> logger) : base(service, logger)
        {
            _userService = service;
        }


        /// <summary>
        /// Crea usuario (Persona + Usuario) y asigna rol explícito si se envía; si no, rol por defecto.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(UserSelectDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public override async Task<ActionResult<UserSelectDto>> Post([FromBody] UserCreateDto dto)
        {
            var created = await _userService.CreateWithPersonAndRolesAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza datos de Persona/Usuario y reemplaza roles. Password es opcional.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(UserSelectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public override async Task<ActionResult<UserSelectDto>> Put(int id, [FromBody] UserUpdateDto dto)
        {
            // Si tu DTO es 'record' con init, puedes usar: dto = dto with { Id = id };
            dto.Id = id; // para clase con set

            var updated = await _userService.UpdateWithPersonAndRolesAsync(dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        // GET, GET/{id}, DELETE, PATCH/soft-delete, PATCH/estado
        // quedan tal como los implementa BaseController<TGet,TCreate,TUpdate>.
    }
}
