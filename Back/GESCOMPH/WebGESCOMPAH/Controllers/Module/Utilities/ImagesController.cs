using Business.Interfaces.Implements.Utilities;
using Entity.DTOs.Implements.Utilities.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPH.Controllers.Module.Utilities
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public sealed class ImagesController : ControllerBase
    {
        private readonly IImagesService _imagesService;

        public ImagesController(IImagesService imagesService)
        {
            _imagesService = imagesService;
        }

        /// <summary>
        /// Adjunta imágenes a un establecimiento existente.
        /// </summary>
        /// <param name="establishmentId">ID del establecimiento</param>
        /// <param name="files">Colección de archivos (1..N)</param>
        [HttpPost("{establishmentId:int}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(List<ImageSelectDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<ImageSelectDto>>> Upload(
            int establishmentId,
            [FromForm] IFormFileCollection files)
        {
            if (files is null || files.Count == 0)
                return BadRequest(new { detail = "Debe adjuntar al menos un archivo." });

            var result = await _imagesService.AddImagesAsync(establishmentId, files);
            return Ok(result);
        }

        /// <summary>
        /// Elimina una imagen por su PublicId (Cloudinary).
        /// </summary>
        [HttpDelete("{publicId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(string publicId)
        {
            await _imagesService.DeleteByPublicIdAsync(publicId);
            return NoContent();
        }

        /// <summary>
        /// Obtiene todas las imágenes asociadas a un establecimiento.
        /// </summary>
        [HttpGet("{establishmentId:int}")]
        [ProducesResponseType(typeof(List<ImageSelectDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ImageSelectDto>>> GetByEstablishment(int establishmentId)
        {
            var images = await _imagesService.GetImagesByEstablishmentIdAsync(establishmentId);
            return Ok(images);
        }


        // DELETE por ID numérico (recomendado)
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteById(int id, CancellationToken ct)
        {
            await _imagesService.DeleteByIdAsync(id);
            return NoContent();
        }

    }
}
