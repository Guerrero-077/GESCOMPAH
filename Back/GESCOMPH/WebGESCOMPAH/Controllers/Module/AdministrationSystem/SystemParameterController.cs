using Business.Interfaces.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.SystemParameter;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPH.Controllers.Base;

namespace WebGESCOMPH.Controllers.Module.AdministrationSystem
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemParameterController : BaseController<SystemParameterSelectDto, SystemParameterDto, SystemParameterUpdateDto>
    {
        public SystemParameterController(ISystemParameterService service, ILogger<SystemParameterController> logger) : base(service, logger)
        {
        }
    }
}
