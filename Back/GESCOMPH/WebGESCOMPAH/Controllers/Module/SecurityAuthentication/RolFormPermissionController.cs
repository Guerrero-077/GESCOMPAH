using Business.Interfaces.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.SecurityAuthentication
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RolFormPermissionController
        : BaseController<RolFormPermissionSelectDto, RolFormPermissionCreateDto, RolFormPermissionUpdateDto>
    {
        private readonly IRolFormPermissionService _rolFormPermissionService;
        private readonly ILogger<RolFormPermissionController> _logger;

        public RolFormPermissionController(
            IRolFormPermissionService service,
            ILogger<RolFormPermissionController> logger
        ) : base(service, logger)
        {
            _rolFormPermissionService = service;
            _logger = logger;
        }

        // GET: api/RolFormPermission/grouped
        [HttpGet("grouped")]
        public async Task<IActionResult> GetAllGrouped()
        {
            try
            {
                var result = await _rolFormPermissionService.GetAllGroupedAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los permisos de rol-formulario agrupados.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno en el servidor.");
            }
        }

        // DELETE: api/RolFormPermission/group/{rolId}/{formId}
        [HttpDelete("group/{rolId:int}/{formId:int}")]
        public async Task<IActionResult> DeleteByGroup([FromRoute] int rolId, [FromRoute] int formId)
        {
            try
            {
                var result = await _rolFormPermissionService.DeleteByGroupAsync(rolId, formId);
                if (!result)
                {
                    return NotFound("No se encontró el grupo de permisos especificado.");
                }
                return NoContent(); // estándar para eliminación exitosa
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el grupo de permisos para rol {RolId} y formulario {FormId}.", rolId, formId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno en el servidor.");
            }
        }
    }
}
