using Business.Interfaces.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.Form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPH.Controllers.Base;

namespace WebGESCOMPH.Controllers.Module.AdministrationSystem
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FormController : BaseController<FormSelectDto, FormCreateDto, FormUpdateDto>
    {
        public FormController(IFormService service, ILogger<FormController> logger) : base(service, logger)
        {
        }
    }
}
