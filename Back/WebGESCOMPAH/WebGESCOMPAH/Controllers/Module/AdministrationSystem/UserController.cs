using Business.CQRS.SecrutityAuthentication.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.AdministrationSystem
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await _mediator.Send(new GetAllUserQuery());
            return Ok(user);
        }
    }
}
