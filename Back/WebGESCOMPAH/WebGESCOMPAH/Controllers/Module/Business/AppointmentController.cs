using Business.Interfaces.Implements.Business;
using Business.Interfaces.Implements.Location;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.Business
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AppointmentController : Controller
    {

        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("Department")]

        public async Task<IActionResult> Get()
        {
            var appointments = await _appointmentService.GetAllAsync();
            return Ok(appointments);
        }
    }
}
