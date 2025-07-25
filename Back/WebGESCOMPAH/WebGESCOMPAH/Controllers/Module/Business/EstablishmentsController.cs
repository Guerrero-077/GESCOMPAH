using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.Establishment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
//[Authorize]
[ApiController]
public class EstablishmentsController : ControllerBase
{
    private readonly IEstablishmentService _establishmentService;
    public EstablishmentsController(IEstablishmentService establishmentService)
    {
        _establishmentService = establishmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _establishmentService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _establishmentService.GetByIdAsync(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] EstablishmentCreateDto dto)
    {
        var result = await _establishmentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromForm] EstablishmentUpdateDto dto)
    {
        var result = await _establishmentService.UpdateAsync(dto);
        return Ok(result);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, [FromQuery] bool forceDelete = false)
    {
        await _establishmentService.DeleteAsync(id, forceDelete);
        return NoContent();
    }


    /// <summary>
    /// Elimina una imagen específica de un establecimiento
    /// </summary>
    [HttpDelete("{establishmentId}/images/{imageId}")]
    public async Task<IActionResult> DeleteImage(int establishmentId, int imageId)
    {
        await _establishmentService.DeleteImageAsync(establishmentId, imageId);
        return NoContent(); // 204
    }
}
