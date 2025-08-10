using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.EstablishmentDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.Business
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class EstablishmentsController : ControllerBase
    {
        private readonly IEstablishmentService _establishmentService;
        public EstablishmentsController(IEstablishmentService establishmentService)
        {
            _establishmentService = establishmentService;
            
        }

        /// <summary>
        /// Obtener todos los establecimientos activos.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EstablishmentSelectDto>>> GetAll()
        {
            var result = await _establishmentService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Obtener un establecimiento por ID.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _establishmentService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Crear un nuevo establecimiento con imágenes.
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<EstablishmentSelectDto>> Create([FromForm] EstablishmentCreateDto dto)
        {
            var result = await _establishmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        /// <summary>
        /// Actualizar un establecimiento y sus imágenes.
        /// </summary>
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<EstablishmentSelectDto>> Update(int id, [FromForm] EstablishmentUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("El ID de la URL no coincide con el ID del cuerpo del formulario.");

            var result = await _establishmentService.UpdateAsync(dto);
            return Ok(result);
        }


        /// <summary>
        /// Eliminar lógicamente un establecimiento (soft delete).
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _establishmentService.DeleteAsync(id);
            return NoContent();
        }
    }
}
