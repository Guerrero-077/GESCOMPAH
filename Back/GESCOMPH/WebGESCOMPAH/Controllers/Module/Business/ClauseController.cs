using Business.Interfaces.IBusiness;
using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.Clause;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPH.Controllers.Base;

namespace WebGESCOMPH.Controllers.Module.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClauseController : BaseController<ClauseSelectDto, ClauseDto, ClauseUpdateDto>
    {
        public ClauseController(IClauseService service, ILogger<ClauseController> logger) : base(service, logger)
        {
        }
    }
}
