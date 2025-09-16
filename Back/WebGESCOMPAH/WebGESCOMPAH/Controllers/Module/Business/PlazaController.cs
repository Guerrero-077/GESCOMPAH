using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.Plaza;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;
using Utilities.Exceptions;
namespace WebGESCOMPAH.Controllers.Module.Business
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PlazaController : BaseController<PlazaSelectDto, PlazaCreateDto, PlazaUpdateDto>
    {

        private readonly IPlazaService _plazaService;

        public PlazaController(IPlazaService service, ILogger<PlazaController> logger) : base(service, logger)
        {
            _plazaService = service;
        }

        [HttpPatch("{id:int}/estado")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public override async Task<IActionResult> ChangeActiveStatus(int id, [FromBody] WebGESCOMPAH.Contracts.Requests.ChangeActiveStatusRequest body)
        {
            try
            {
                await _plazaService.UpdateActiveStatusAsync(id, body.Active!.Value);
                return NoContent();
            }
            catch (BusinessException ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
