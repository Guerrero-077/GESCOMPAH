using Business.Interfaces.Implements.Utilities;
using Entity.DTOs.Implements.Utilities.Images;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.Utilities
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImagesService _imagesService;

        public ImagesController(IImagesService imagesService)
        {
            _imagesService = imagesService;
        }

        /// <summary>
        /// Obtener todas las imágenes asociadas a un establecimiento
        /// </summary>
        [HttpGet("establishment/{establishmentId}")]
        public async Task<ActionResult<List<ImageSelectDto>>> GetImagesByEstablishmentId(int establishmentId)
        {
            var result = await _imagesService.GetImagesByEstablishmentIdAsync(establishmentId);
            return Ok(result);
        }

        /// <summary>
        /// Subir imágenes a un establecimiento (máximo 5 por establecimiento)
        /// </summary>
        [HttpPost("establishment/{establishmentId}")]
        public async Task<ActionResult<List<ImageSelectDto>>> UploadImages(
            int establishmentId,
            [FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("Debe subir al menos una imagen.");

            var result = await _imagesService.AddImagesAsync(establishmentId, files);
            return CreatedAtAction(nameof(GetImagesByEstablishmentId), new { establishmentId }, result);
        }

        /// <summary>
        /// Eliminar una imagen por su ID
        /// </summary>
        [HttpDelete("{imageId:int}")]
        public async Task<IActionResult> DeleteImageById(int imageId)
        {
            await _imagesService.DeleteImageByIdAsync(imageId);
            return NoContent();
        }

        /// <summary>
        /// Eliminar múltiples imágenes usando una lista de publicIds
        /// </summary>
        [HttpDelete("by-publicIds")]
        public async Task<IActionResult> DeleteImagesByPublicIds([FromBody] List<string> publicIds)
        {
            if (publicIds == null || !publicIds.Any())
                return BadRequest("Debe proporcionar al menos un publicId.");

            await _imagesService.DeleteImagesByPublicIdsAsync(publicIds);
            return NoContent();
        }
    }
}
