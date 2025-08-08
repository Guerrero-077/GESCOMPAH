using Business.Interfaces.Implements.Location;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Location.Department;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.Locations
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class DepartmentController : BaseController<DepartmentSelectDto, DepartmentCreateDto, DepartmentUpdateDto, IDepartmentService>
    {
        public DepartmentController(IDepartmentService service, ILogger<DepartmentController> logger) : base(service, logger)
        {
        }

        protected override async Task AddAsync(DepartmentCreateDto dto)
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

        protected override async Task<IEnumerable<DepartmentSelectDto>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        protected override async Task<DepartmentSelectDto?> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        protected override async Task<DepartmentSelectDto> UpdateAsync(int id, DepartmentUpdateDto dto)
        {
            return await _service.UpdateAsync(dto);
        }
    }
}
