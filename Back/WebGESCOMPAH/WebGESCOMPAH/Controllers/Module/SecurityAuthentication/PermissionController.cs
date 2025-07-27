using Business.Interfaces.Implements.SecrutityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Permission;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebGESCOMPAH.Controllers.Module.SecurityAuthentication
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]

    public class PermissionController(IPermissionService permissionService) : ControllerBase
    {

        private readonly IPermissionService _permissionService = permissionService;

        // GET: api/<PermissionController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
           var permissions = await _permissionService.GetAllAsync();
            return Ok(permissions);
        }

        // GET api/<PermissionController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var permission = await _permissionService.GetByIdAsync(id);
            return Ok(permission);
        }

        // POST api/<PermissionController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PermissionCreateDto permissionDto)
        {
            var permission = await _permissionService.CreateAsync(permissionDto);
            return CreatedAtAction(nameof(Get), new { id = permission.Id }, permission);
        }

        // PUT api/<PermissionController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] PermissionUpdateDto permissionDto)
        {

            var existingPermission = await _permissionService.GetByIdAsync(id);

            await _permissionService.UpdateAsync(permissionDto);
            return NoContent();
        }

        // DELETE api/<PermissionController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingPermission = await _permissionService.GetByIdAsync(id);
            if (existingPermission == null)
            {
                return NotFound();
            }

            await _permissionService.DeleteAsync(id);
            return Ok(new { message = $"El Permission con ID {id} fue eliminado exitosamente." });

        }
    }
}
