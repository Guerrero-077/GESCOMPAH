using Business.Interfaces.IBusiness;
using Business.Interfaces.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.Form;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.AdministrationSystem
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormController : BaseController<FormSelectDto, FormCreateDto, FormUpdateDto, IFormService>
    {
        public FormController(IFormService service, ILogger<FormController> logger) : base(service, logger)
        {
        }

        protected override async Task AddAsync(FormCreateDto dto)
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

        protected override async Task<IEnumerable<FormSelectDto>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        protected override async Task<FormSelectDto?> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        protected override async Task<FormUpdateDto> UpdateAsync(int id, FormUpdateDto dto)
        {
            return await _service.UpdateAsync(dto);
        }
    }
}
