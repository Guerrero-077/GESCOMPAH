using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.EstablishmentDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.Business
{
    [ApiController]
    [Authorize] // [Authorize(Policy = "CanManageEstablishments")] si usas policies
    [Route("api/[controller]")]
    [Produces("application/json")]
    public sealed class EstablishmentsController : ControllerBase
    {
        private readonly IEstablishmentService _establishmentService;

        public EstablishmentsController(IEstablishmentService establishmentService)
        {
            _establishmentService = establishmentService;
        }

        /// <summary>Obtener todos los establecimientos.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EstablishmentSelectDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EstablishmentSelectDto>>> GetAll()
        {
            var result = await _establishmentService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>Obtener un establecimiento por ID.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EstablishmentSelectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _establishmentService.GetByIdAsync(id);
            if (result is null) return NotFound();
            return Ok(result);
        }

        /// <summary>Crear un establecimiento (JSON puro; SIN imágenes).</summary>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(EstablishmentSelectDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EstablishmentSelectDto>> Create([FromBody] EstablishmentCreateDto dto)
        {
            // [ApiController] validará el modelo automáticamente; si quieres validaciones custom, hazlas en el servicio.
            var result = await _establishmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>Actualizar un establecimiento (JSON puro; imágenes aparte).</summary>
        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(EstablishmentSelectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EstablishmentSelectDto>> Update(int id, [FromBody] EstablishmentUpdateDto dto)
        {
            if (id <= 0)
                return BadRequest(new { id = new[] { "El Id debe ser mayor a 0." } });

            // La ruta manda
            dto.Id = id;

            var updated = await _establishmentService.UpdateAsync(dto);
            if (updated is null) return NotFound();

            return Ok(updated);
        }

        /// <summary>Eliminar (borrado lógico).</summary>
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
        /// (Opcional) Obtener datos básicos por IDs (para sumatorias: RentValueBase, UvtQty).
        /// Útil si lo necesitas desde front u otra integración. Si solo lo usas internamente, puedes omitir este endpoint.
        /// </summary>
        [HttpPost("basics")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBasics([FromBody] int[] ids)
        {
            if (ids is null || ids.Length == 0)
                return BadRequest(new { ids = new[] { "Debe enviar al menos un ID de establecimiento." } });

            // Exponemos como objeto anónimo liviano para no acoplar con entidades internas
            var basics = await _establishmentService.GetBasicsByIdsAsync(ids);
            var dto = basics.Select(b => new
            {
                b.Id,
                RentValueBase = b.RentValueBase,
                UvtQty = b.UvtQty
            });

            return Ok(dto);
        }
    }
}
