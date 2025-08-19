using Business.Interfaces.Implements.Business;
using Entity.DTOs.Implements.Business.Plaza;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;
namespace WebGESCOMPAH.Controllers.Module.Business
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PlazaController : BaseController<PlazaSelectDto, PlazaCreateDto, PlazaUpdateDto>
    {

        private readonly IPlazaService _plazaService;

        public PlazaController(IPlazaService service, ILogger<PlazaController> logger) : base(service, logger)
        {
            _plazaService = service;
        }




    }
}
