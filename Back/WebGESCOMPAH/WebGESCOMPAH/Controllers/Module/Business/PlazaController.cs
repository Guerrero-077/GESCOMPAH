using Business.CQRS.Business.Plaza;
using Business.CQRS.Business.Plaza.Create;
using Business.CQRS.Business.Plaza.Select;
using Business.CQRS.Business.Plaza.Update;
using Entity.DTOs.Implements.Business.Plaza;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebGESCOMPAH.Controllers.Module.Business
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PlazaController : ControllerBase
    {

        private readonly IMediator _mediator;

        public PlazaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Listar todas nueva Plaza
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var plazas = await _mediator.Send(new GetAllPlazasQuery());
            return Ok(plazas);
        }
        /// <summary>
        /// Listar una nueva Plaza por ID
        /// </summary>

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var plaza = await _mediator.Send(new GetPlazaByIdQuery(id));

            if (plaza == null)
                return NotFound();

            return Ok(plaza);
        }

        /// <summary>
        /// Crea una nueva Plaza
        /// </summary>
        [HttpPost("CrearPlaza")]
        public async Task<IActionResult> Create([FromBody] PlazaCreateDto plazaDto)
        {
            var command = new CreatePlazaCommand(plazaDto);
            var newPlazaId = await _mediator.Send(command);

            // Retorna 201 Created sin CreatedAtAction ni GetById
            return StatusCode(StatusCodes.Status201Created, new { id = newPlazaId });
        }

        /// <summary>
        /// Actualiza una plaza existente
        /// </summary>
        [HttpPut("UpdatePlaza")]
        public async Task<IActionResult> Update([FromBody] PlazaUpdateDto plazaDto)
        {
           
            var command = new UpdatePlazaCommand(plazaDto);
            var updatedPlaza = await _mediator.Send(command);

            return Ok(updatedPlaza);
        }

        /// <summary>
        /// Actualiza el estado de una plaza existente por ID
        /// </summary>

        [HttpPatch("{id:int}/estado")]
        public async Task<IActionResult> ChangeActiveStatus(int id, [FromBody] bool active)
        {
            var result = await _mediator.Send(new UpdatePlazaActiveStatusCommand(id, active));
            return Ok(result);
        }




    }
}
