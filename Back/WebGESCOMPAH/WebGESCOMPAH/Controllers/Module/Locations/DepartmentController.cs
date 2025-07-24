using Business.CQRS.Location.Department.Select;
using Business.CQRS.Persons.Person.Select;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.Locations
{
    [ApiController]
    //[Authorize]
    [Route("api/v1/[controller]")]
    public class DepartmentController : Controller
    {
        private readonly IMediator _mediator;

        public DepartmentController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("Department")]

        public async Task<IActionResult> Get()
        {
            var person = await _mediator.Send(new GetAllDepartmentQuery());
            return Ok(person);
        }
    }
}
