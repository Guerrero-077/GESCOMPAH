using Business.Interfaces.Implements.Persons;
using Entity.DTOs.Implements.Persons.Peron;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.Persons
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PersonController : BaseController<PersonSelectDto, PersonDto, PersonUpdateDto>
    {
        public PersonController(IPersonService service, ILogger<PersonController> logger) : base(service, logger)
        {
        }
    }
}
