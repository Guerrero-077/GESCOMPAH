using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.EstablishmentDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.Business
{
    [ApiController]
    [Authorize] // [Authorize(Policy = "CanManageEstablishments")]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public sealed class EstablishmentsController : ControllerBase
    {
        private readonly IEstablishmentService _establishmentService;

        public EstablishmentsController(IEstablishmentService establishmentService)
        {
            _establishmentService = establishmentService;
        }

        /// <summary>
        /// Obtener establecimientos. 
        /// Usa ?activeOnly=true para traer solo activos; por defecto trae todos (activos e inactivos). Usa ?limit=10 para limitar la cantidad de registros.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EstablishmentSelectDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EstablishmentSelectDto>>> GetAll([FromQuery] bool activeOnly = false, [FromQuery] int? limit = null)
        {
            var result = activeOnly
                ? await _establishmentService.GetAllActiveAsync(limit)
                : await _establishmentService.GetAllAnyAsync(limit);

            return Ok(result);
        }

        /// <summary>
        /// Obtener establecimientos de una plaza. Usa ?activeOnly=true para traer solo activos y ?limit=10 para limitar resultados.
        /// </summary>
        [HttpGet("plaza/{plazaId:int}")]
        [ProducesResponseType(typeof(IEnumerable<EstablishmentSelectDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<EstablishmentSelectDto>>> GetByPlaza(int plazaId, [FromQuery] bool activeOnly = false, [FromQuery] int? limit = null)
        {
            if (plazaId <= 0)
                return BadRequest(new { plazaId = new[] { "El plazaId debe ser mayor a 0." } });

            var result = await _establishmentService.GetByPlazaIdAsync(plazaId, activeOnly, limit);
            return Ok(result);
        }


        /// <summary>
        /// Listado liviano para tarjetas/grillas. Incluye PrimaryImagePath pero no la coleccion completa de imagenes.
        /// </summary>
        [HttpGet("cards")]
        [ProducesResponseType(typeof(IEnumerable<EstablishmentCardDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCards([FromQuery] bool activeOnly = false)
        {
            var result = activeOnly
                ? await _establishmentService.GetCardsActiveAsync()
                : await _establishmentService.GetCardsAnyAsync();
            return Ok(result);
        }

        /// <summary>
        /// Obtener un establecimiento por ID.
        /// Usa ?activeOnly=true para forzar que sea activo; por defecto busca cualquiera.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EstablishmentSelectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, [FromQuery] bool activeOnly = false)
        {
            var result = activeOnly
                ? await _establishmentService.GetByIdActiveAsync(id)
                : await _establishmentService.GetByIdAnyAsync(id);

            if (result is null) return NotFound();
            return Ok(result);
        }

        /// <summary>Crear un establecimiento (JSON puro; SIN imagenes).</summary>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(EstablishmentSelectDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EstablishmentSelectDto>> Create([FromBody] EstablishmentCreateDto dto)
        {
            var result = await _establishmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>Actualizar un establecimiento (JSON puro; imagenes aparte).</summary>
        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(EstablishmentSelectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EstablishmentSelectDto>> Update(int id, [FromBody] EstablishmentUpdateDto dto)
        {
            if (id <= 0)
                return BadRequest(new { id = new[] { "El Id debe ser mayor a 0." } });

            dto.Id = id; // la ruta manda

            var updated = await _establishmentService.UpdateAsync(dto);
            if (updated is null) return NotFound();

            return Ok(updated);
        }

        /// <summary>Eliminar (borrado logico).</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _establishmentService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// (Opcional) Obtener datos basicos por IDs para sumatorias (RentValueBase, UvtQty).
        /// </summary>
        [HttpPost("basics")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBasics([FromBody] int[] ids)
        {
            if (ids is null || ids.Length == 0)
                return BadRequest(new { ids = new[] { "Debe enviar al menos un ID de establecimiento." } });

            var basics = await _establishmentService.GetBasicsByIdsAsync(ids);
            var dto = basics.Select(b => new { b.Id, b.RentValueBase, b.UvtQty });
            return Ok(dto);
        }
    }
}
