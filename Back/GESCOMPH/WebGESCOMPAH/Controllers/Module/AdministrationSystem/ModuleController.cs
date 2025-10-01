using Business.Interfaces.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPH.Controllers.Base;

namespace WebGESCOMPH.Controllers.Module.AdministrationSystem
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ModuleController : BaseController<ModuleSelectDto, ModuleCreateDto, ModuleUpdateDto>
    {
        public ModuleController(IModuleService service, ILogger<ModuleController> logger) : base(service, logger)
        {
        }
    }
}
