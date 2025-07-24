using Business.CQRS.Utilities.Image.Select;
using Business.Interfaces.Implements;
using Entity.DTOs.Implements.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.Utilities
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class ImagesController : Controller
    {
        private readonly IMediator _mediaror;
        public ImagesController(IMediator mediaror)
        {
            _mediaror = mediaror;
        }

        [HttpGet("Images")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var images = await _mediaror.Send(new GetAllImageQuery());
                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener las imágenes: {ex.Message}");
            }
        }
    }
}