using Business.Interfaces.Implements.Location;
using Entity.DTOs.Implements.Location.Department;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.Locations
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class DepartmentController : BaseController<DepartmentSelectDto, DepartmentCreateDto, DepartmentUpdateDto>
    {
        public DepartmentController(IDepartmentService service, ILogger<DepartmentController> logger) : base(service, logger)
        {
        }
    }
}
