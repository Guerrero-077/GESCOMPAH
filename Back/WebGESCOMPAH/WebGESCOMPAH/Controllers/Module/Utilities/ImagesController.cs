using Business.Interfaces.Implements.Utilities;
using Business.Services.Utilities;
using Entity.DTOs.Implements.Utilities.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers.Utilities;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IImagesService _imagesService;
    private readonly ILogger _logger;

    public ImagesController(IImagesService imagesService, ILogger<ImagesController> logger)
    {
        _imagesService = imagesService;
        _logger = logger;
    }

    /// <summary>
    /// Subir nuevas imágenes para un establecimiento (máx. 5 en total)
    /// </summary>
    [HttpPost("{establishmentId}")]
    public async Task<IActionResult> UploadImages(int establishmentId, [FromForm] IFormFileCollection files)
    {
        if (files == null || files.Count == 0)
            return BadRequest("No se han proporcionado archivos.");

        var result = await _imagesService.AddImagesAsync(establishmentId, files);
        return Ok(result);
    }

    /// <summary>
    /// Reemplazar una imagen específica
    /// </summary>
    //[HttpPut("{imageId}")]
    //public async Task<IActionResult> ReplaceImage([FromForm] ImageUpdateDto dto)
    //{
    //    if (dto.File == null)
    //        return BadRequest("Debe proporcionar un archivo para reemplazo.");

    //    var updatedImage = await _imagesService.ReplaceImageAsync(dto.Id, dto.File);
    //    return Ok(updatedImage);
    //}

    /// <summary>
    /// Eliminar una imagen por su ID
    /// </summary>
    [HttpDelete("{imageId}")]
    public async Task<IActionResult> DeleteImage(int imageId)
    {
        await _imagesService.DeleteImageByIdAsync(imageId);
        return NoContent();
    }

    [HttpPatch("logical-delete")]
    public async Task<IActionResult> LogicalDelete([FromQuery] string publicId)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            return BadRequest("El publicId es obligatorio.");

        var result = await _imagesService.DeleteLogicalByPublicIdAsync(publicId);

        if (!result)
            return NotFound($"No se encontró la imagen con publicId '{publicId}'.");

        return NoContent();
    }



    /// <summary>
    /// Obtener todas las imágenes de un establecimiento
    /// </summary>
    [HttpGet("{establishmentId}")]
    public async Task<ActionResult<List<ImageSelectDto>>> GetByEstablishment(int establishmentId)
    {
        var images = await _imagesService.GetImagesByEstablishmentIdAsync(establishmentId);
        return Ok(images);
    }
}
