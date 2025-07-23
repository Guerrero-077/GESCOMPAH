using Business.CQRS.Auth.Query.Persons;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module
{
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
