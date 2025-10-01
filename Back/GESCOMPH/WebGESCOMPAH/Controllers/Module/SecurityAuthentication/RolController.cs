using Business.Interfaces.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPH.Controllers.Base;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]


public class RolController : BaseController<RolSelectDto, RolCreateDto, RolUpdateDto>
{
    public RolController(IRolService service, ILogger<RolController> logger) : base(service, logger)
    {
    }


}
