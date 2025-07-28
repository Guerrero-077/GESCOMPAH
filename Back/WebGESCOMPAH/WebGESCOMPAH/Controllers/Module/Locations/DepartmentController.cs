using Business.Interfaces.Implements.Location;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Location.Department;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.Locations
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<DepartmentSelectDto>>> Get()
        {
            var departments = await _departmentService.GetAllAsync();
            return Ok(departments);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DepartmentSelectDto>> GetById(int id)
        {
            var departments = await _departmentService.GetByIdAsync(id);
            if (departments == null)
                return NotFound($"Department con ID {id} no encontrado.");
            return Ok(departments);
        }

        [HttpPost]
        public async Task<ActionResult<DepartmentSelectDto>> Create([FromForm] DepartmentCreateDto dto) 
        {
            var departaments = await _departmentService.CreateAsync(dto);
            return Ok(departaments);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id,  [FromBody] DepartmentUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("ID en URL no coincide con el del DTO.");
            }
            var departments = await _departmentService.UpdateAsync(dto);
            return Ok(departments);
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _departmentService.DeleteAsync(id);
            return Ok();
        }

    }
}
