using Business.Interfaces.Implements.Utilities;
using Entity.DTOs.Implements.Utilities.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.Utilities
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImagesService _imagesService;

        public ImagesController(IImagesService imagesService)
        {
            _imagesService = imagesService;
        }

        [HttpGet("Images")]
        public async Task<IActionResult> Get()
        {
            var images = await _imagesService.GetAllAsync();
            return Ok(images);
        }

        [HttpGet("ImagesByEstablishmentId/{establishmentId}")]
        public async Task<IActionResult> GetByEstablishmentId(int establishmentId)
        {
            var images = await _imagesService.GetByEstablishmentIdAsync(establishmentId);
            return Ok(images);
        }

        [HttpPost("AddImages")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddImages([FromForm] ImageCreateDto images)
        {
            if (images == null || images.Files == null || !images.Files.Any())
                return BadRequest("La lista de imágenes no puede ser nula o vacía.");

            await _imagesService.AddAsync(images);
            return Ok("Imágenes agregadas correctamente.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateImage(int id, [FromForm] ImageUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var updated = await _imagesService.UpdateAsync(dto);
            return Ok(updated);
        }

        [HttpDelete("DeleteImage/{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var deleted = await _imagesService.DeleteAsync(id);
            return deleted
                ? Ok("Imagen eliminada correctamente.")
                : NotFound("Imagen no encontrada.");
        }
    }
}
