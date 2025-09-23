using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.Appointment;
using Entity.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.Business
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class AppointmentController : Controller
    {

        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]

        public async Task<IActionResult> Get()
        {
            var appointments = await _appointmentService.GetAllAsync();
            return Ok(appointments);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AppointmentSelectDto>> GetById(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);

            if (appointment == null)
                return NotFound($"Appointment con Id {id} no encontrado");

            return Ok(appointment);
        }

        [HttpPost]
        public async Task<ActionResult<AppointmentSelectDto>> Create([FromBody] AppointmentCreateDto dto)
        {
            var appointment = await _appointmentService.CreateAsync(dto);
            return Ok(appointment);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<AppointmentSelectDto>> Update(int id, [FromBody] AppointmentUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID en URL no coincide con el DTO");

            var appointment = await _appointmentService.UpdateAsync(dto);
            return Ok(appointment);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _appointmentService.DeleteAsync(id);
            return Ok();
        }

    }
}
