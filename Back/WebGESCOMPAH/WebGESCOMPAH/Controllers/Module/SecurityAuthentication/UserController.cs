using Business.Interfaces.IBusiness;
using Business.Interfaces.Implements.SecrutityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.SecurityAuthentication
{
    [ApiController]
    [Authorize]
    [Produces("application/json")]

    [Route("api/[controller]")]
    public class UserController : BaseController<UserSelectDto, UserCreateDto, UserUpdateDto>
    {
        public UserController(IUserService service, ILogger<UserController> logger) : base(service, logger)
        {
        }
    }
}
