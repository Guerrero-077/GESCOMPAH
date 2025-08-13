using Business.Interfaces.Implements.Location;
using Entity.DTOs.Implements.Location.City;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.Locations
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CityController : BaseController<CitySelectDto, CityCreateDto, CityUpdateDto, ICityService>
    {
        public CityController(ICityService service, ILogger<CityController> logger) : base(service, logger)
        {
        }
        protected override async Task<IEnumerable<CitySelectDto>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        protected override async Task<CitySelectDto?> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }


        [HttpGet("Department/City/{id}")]
        //[ProducesResponseType(typeof(TDto), 200)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public virtual async Task<IActionResult> GetCityWhiteDepartament(int id)
        {
            try
            {
                var result = await _service.GetCityByDepartment(id);
                if (result == null)
                    return NotFound(new { message = $"No se encontró el elemento con ID {id}" });

                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor." });
            }
        }

        protected override async Task AddAsync(CityCreateDto dto)
        {
            await _service.CreateAsync(dto);
        }
        protected override async Task<CitySelectDto> UpdateAsync(int id, CityUpdateDto dto)
        {
            return await _service.UpdateAsync(dto);
        }

        protected override async Task<bool> DeleteAsync(int id)
        {
            return await _service.DeleteAsync(id);
        }

        protected override async Task<bool> DeleteLogicAsync(int id)
        {
            return await _service.DeleteLogicAsync(id);
        }


    }
}
