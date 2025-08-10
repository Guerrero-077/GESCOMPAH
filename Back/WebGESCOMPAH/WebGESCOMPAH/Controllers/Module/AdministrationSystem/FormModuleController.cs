using Business.Interfaces.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.FormModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.AdministrationSystem
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FormModuleController : BaseController<FormModuleSelectDto, FormModuleCreateDto, FormModuleUpdateDto, IFormMouduleService>
    {
        public FormModuleController(IFormMouduleService service, ILogger<FormModuleController> logger) : base(service, logger)
        {
        }

        protected override async Task AddAsync(FormModuleCreateDto dto)
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

        protected override async Task<IEnumerable<FormModuleSelectDto>> GetAllAsync()
        {
           return  await _service.GetAllAsync();
        }

        protected override async Task<FormModuleSelectDto?> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        protected override async Task<FormModuleSelectDto> UpdateAsync(int id, FormModuleUpdateDto dto)
        {
            return await _service.UpdateAsync(dto);
        }
    }
}
