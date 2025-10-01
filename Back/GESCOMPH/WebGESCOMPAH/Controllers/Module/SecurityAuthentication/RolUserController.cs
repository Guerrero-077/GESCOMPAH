using Business.Interfaces.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.RolUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPH.Controllers.Base;

namespace WebGESCOMPH.Controllers.Module.SecurityAuthentication
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RolUserController : BaseController<RolUserSelectDto, RolUserCreateDto, RolUserUpdateDto>
    {
        public RolUserController(IRolUserService service, ILogger<RolUserController> logger) : base(service, logger)
        {
        }
    }
}
