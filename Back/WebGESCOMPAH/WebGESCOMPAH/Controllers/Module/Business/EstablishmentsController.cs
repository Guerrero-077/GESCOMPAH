using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.EstablishmentDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.Business
{
    [ApiController]
    [Authorize] // opcional: [Authorize(Policy = "CanManageEstablishments")]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EstablishmentsController : ControllerBase
    {
        private readonly IEstablishmentService _establishmentService;

        public EstablishmentsController(IEstablishmentService establishmentService)
        {
            _establishmentService = establishmentService;
        }

        /// <summary>Obtener todos los establecimientos activos (considera agregar paginación).</summary>
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
            if (result is null) return NotFound(); // ← 404 explícito (si tu servicio devuelve null)
            return Ok(result);
        }

        /// <summary>Crear un nuevo establecimiento con imágenes.</summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestFormLimits(MultipartBodyLengthLimit = 25_000_000)] // ~25MB (ajusta)
        [RequestSizeLimit(25_000_000)]
        [ProducesResponseType(typeof(EstablishmentSelectDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EstablishmentSelectDto>> Create(
            [FromForm] EstablishmentCreateDto dto)
        {
            // [ApiController] ya valida ModelState 400 automáticamente
            var result = await _establishmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>Actualizar un establecimiento y sus imágenes.</summary>
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        [RequestFormLimits(MultipartBodyLengthLimit = 25_000_000)] // consistente con POST
        [RequestSizeLimit(25_000_000)]
        [ProducesResponseType(typeof(EstablishmentSelectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EstablishmentSelectDto>> Update(
            int id,
            [FromForm] EstablishmentUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("El ID de la URL no coincide con el ID del cuerpo del formulario.");

            var result = await _establishmentService.UpdateAsync(dto);
            if (result is null) return NotFound(); // si tu servicio retorna null si no existe
            return Ok(result);
        }

        /// <summary>Eliminar lógicamente un establecimiento (soft delete).</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _establishmentService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
