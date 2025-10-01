using Business.Interfaces.Implements.Location;
using Entity.DTOs.Implements.Location.City;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebGESCOMPH.Controllers.Base;

namespace WebGESCOMPH.Controllers.Module.Locations
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CityController : BaseController<CitySelectDto, CityCreateDto, CityUpdateDto>
    {
        private readonly ICityService _service;
        public CityController(ICityService service, ILogger<CityController> logger) : base(service, logger)
        {
            _service = service;
        }

        [HttpGet("CityWithDepartment/{id}")]
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
                Logger.LogWarning(ex, "Validación fallida con ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error al obtener el ID {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor." });
            }
        }


    }
}
