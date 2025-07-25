using Business.Interfaces.Implements.Location;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.Locations
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet("Department")]

        public async Task<IActionResult> Get()
        {
            var departments = await _departmentService.GetAllAsync();
            return Ok(departments);
        }
    }
}
