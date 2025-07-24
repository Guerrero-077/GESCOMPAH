using Business.CQRS.Persons.Person;
using Business.CQRS.Persons.Person.Select;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.Persons
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class PersonController : Controller
    {
        private readonly IMediator _mediator;

        public PersonController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("Person")]        
        
        public async Task<IActionResult> Get()
        {
            var person = await _mediator.Send(new GetAllPersonsQuery());
            return Ok(person);
        }

        
    }
}
