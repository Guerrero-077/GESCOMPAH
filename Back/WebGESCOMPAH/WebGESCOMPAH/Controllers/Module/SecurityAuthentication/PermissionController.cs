using Business.Interfaces.Implements.SecrutityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.SecurityAuthentication
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]

    public class PermissionController : BaseController<PermissionSelectDto, PermissionCreateDto, PermissionUpdateDto>
    {
        public PermissionController(IPermissionService service, ILogger<PermissionController> logger) : base(service, logger)
        {
        }
    }
}
