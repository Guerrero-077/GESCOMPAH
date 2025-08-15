using Business.Interfaces.Implements.SecrutityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.RolUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.SecurityAuthentication
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
