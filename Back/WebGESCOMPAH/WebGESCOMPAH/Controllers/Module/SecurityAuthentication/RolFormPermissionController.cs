using Business.Interfaces.IBusiness;
using Business.Interfaces.Implements.SecrutityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.SecurityAuthentication
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RolFormPermissionController : BaseController<RolFormPermissionSelectDto, RolFormPermissionCreateDto, RolFormPermissionUpdateDto>
    {
        public RolFormPermissionController(IRolFormPermissionService service, ILogger<RolFormPermissionController> logger) : base(service, logger)
        {
        }

      
    }
}
