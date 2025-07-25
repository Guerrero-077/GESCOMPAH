using Business.Interfaces.Implements.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.Utilities
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ImagesController : Controller
    {
        private readonly IImagesService _imagesService;
        public ImagesController(IImagesService imagesService)
        {
            _imagesService = imagesService;
        }

        [HttpGet("Images")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var images = await _imagesService.GetAllAsync();
                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener las imágenes: {ex.Message}");
            }
        }
    }
}