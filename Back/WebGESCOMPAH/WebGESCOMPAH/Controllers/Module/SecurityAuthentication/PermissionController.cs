using Business.Interfaces.Implements.SecrutityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Permission;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebGESCOMPAH.Controllers.Module.SecurityAuthentication
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]

    public class PermissionController : BaseController<PermissionSelectDto, PermissionCreateDto, PermissionUpdateDto, IPermissionService>
    {
        public PermissionController(IPermissionService service, ILogger<PermissionController> logger) : base(service, logger)
        {
        }

        protected override async Task AddAsync(PermissionCreateDto dto)
        {
            await _service.CreateAsync(dto);
        }

        protected override async Task<bool> DeleteAsync(int id)
        {
            return await _service.DeleteAsync(id);
        }

        protected override async Task<bool> DeleteLogicAsync(int id)
        {
            return await _service.DeleteLogicAsync(id);
        }

        protected override async Task<IEnumerable<PermissionSelectDto>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        protected override async Task<PermissionSelectDto?> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        protected override async Task<PermissionSelectDto> UpdateAsync(int id, PermissionUpdateDto dto)
        {
            return await _service.UpdateAsync(dto);
        }
    }
}
