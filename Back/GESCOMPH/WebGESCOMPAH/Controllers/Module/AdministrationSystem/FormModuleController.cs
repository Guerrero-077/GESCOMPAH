using Business.Interfaces.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.FormModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPH.Controllers.Base;

namespace WebGESCOMPH.Controllers.Module.AdministrationSystem
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FormModuleController : BaseController<FormModuleSelectDto, FormModuleCreateDto, FormModuleUpdateDto>
    {
        public FormModuleController(IFormMouduleService service, ILogger<FormModuleController> logger) : base(service, logger)
        {
        }
    }
}
