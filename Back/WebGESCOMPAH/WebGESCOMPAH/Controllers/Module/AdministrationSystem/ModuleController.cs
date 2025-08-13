using Business.Interfaces.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.AdministrationSystem
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ModuleController : BaseController<ModuleSelectDto, ModuleCreateDto, ModuleUpdateDto, IModuleService>
    {
        public ModuleController(IModuleService service, ILogger<ModuleController> logger) : base(service, logger)
        {
        }

        protected override async Task AddAsync(ModuleCreateDto dto)
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

        protected override async Task<IEnumerable<ModuleSelectDto>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        protected override async Task<ModuleSelectDto?> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        protected override async Task<ModuleSelectDto> UpdateAsync(int id, ModuleUpdateDto dto)
        {
            return await _service.UpdateAsync(dto);
        }
    }
}
