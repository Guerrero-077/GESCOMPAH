using Business.Interfaces.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPH.Controllers.Base;

namespace WebGESCOMPH.Controllers.Module.SecurityAuthentication
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
