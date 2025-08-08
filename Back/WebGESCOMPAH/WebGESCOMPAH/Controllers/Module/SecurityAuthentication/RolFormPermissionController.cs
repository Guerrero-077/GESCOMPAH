using Business.Interfaces.IBusiness;
using Business.Interfaces.Implements.SecrutityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.SecurityAuthentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolFormPermissionController : BaseController<RolFormPermissionSelectDto, RolFormPermissionCreateDto, RolFormPermissionUpdateDto, IRolFormPermissionService>
    {
        public RolFormPermissionController(IRolFormPermissionService service, ILogger<RolFormPermissionController> logger) : base(service, logger)
        {
        }

        protected override async Task AddAsync(RolFormPermissionCreateDto dto)
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

        protected override async Task<IEnumerable<RolFormPermissionSelectDto>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        protected override async Task<RolFormPermissionSelectDto?> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        protected override async Task<RolFormPermissionSelectDto> UpdateAsync(int id, RolFormPermissionUpdateDto dto)
        {
            return await _service.UpdateAsync(dto);
        }
    }
}
