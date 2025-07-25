using Business.Interfaces.Implements.Persons;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Controllers.Module.Persons
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PersonController : Controller
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }
        [HttpGet("Person")]        
        
        public async Task<IActionResult> Get()
        {
            var person = await _personService.GetAllAsync();
            return Ok(person);
        }

        
    }
}
