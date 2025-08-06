using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.Plaza;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace WebGESCOMPAH.Controllers.Module.Business
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class PlazaController : ControllerBase
    {

        private readonly IPlazaService _plazaService;

        public PlazaController(IPlazaService plazaService)
        {
            _plazaService = plazaService;
        }

        /// <summary>
        /// Listar todas nueva Plaza
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var plazas = await _plazaService.GetAllAsync();
            return Ok(plazas);
        }
        /// <summary>
        /// Listar una nueva Plaza por ID
        /// </summary>

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var plaza = await _plazaService.GetByIdAsync(id);

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
            var newPlazaId = await _plazaService.CreateAsync(plazaDto);

            // Retorna 201 Created sin CreatedAtAction ni GetById
            return StatusCode(StatusCodes.Status201Created, new { id = newPlazaId });
        }

        /// <summary>
        /// Actualiza una plaza existente
        /// </summary>
        [HttpPut("UpdatePlaza")]
        public async Task<IActionResult> Update([FromBody] PlazaUpdateDto plazaDto)
        {

            var updatedPlaza = await _plazaService.UpdateAsync(plazaDto);

            return Ok(updatedPlaza);
        }

        /// <summary>
        /// Actualiza el estado de una plaza existente por ID
        /// </summary>

        [HttpPatch("{id:int}/estado")]
        public async Task<IActionResult> ChangeActiveStatus(int id, [FromBody] bool active)
        {
            var result = await _plazaService.UpdateActiveStatusAsync(id, active);
            return Ok(result);
        }




    }
}
