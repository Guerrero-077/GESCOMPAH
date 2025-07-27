using Business.Interfaces.Implements.AdministrationSystem;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
//[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]


public class RolController(IRolService rolService) : ControllerBase
{
    private readonly IRolService _rolService = rolService;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var roles = await _rolService.GetAllAsync();
        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var rol = await _rolService.GetByIdAsync(id);
        return Ok(rol); // Si no existe, debería lanzar EntityNotFoundException
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RolCreateDto rolDto)
    {
        var result = await _rolService.CreateAsync(rolDto);
        return Ok(new { message = "Elemento agregado exitosamente" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] RolUpdateDto rolDto)
    {
        if (rolDto == null || id != rolDto.Id)
            return BadRequest("Los IDs no coinciden o el cuerpo está mal formado.");

        var existingRol = await _rolService.GetByIdAsync(id);
        if (existingRol == null)
            return NotFound($"No se encontró un rol con ID {id}.");

        var result = await _rolService.UpdateAsync(rolDto);
        return Ok(new { message = "Elemento Actualizado exitosamente" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _rolService.DeleteAsync(id);
        return Ok(new { message = $"El rol con ID {id} fue eliminado exitosamente." });

    }
}
