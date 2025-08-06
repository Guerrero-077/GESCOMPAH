using Business.Interfaces.Implements.Location;
using Entity.DTOs.Implements.Location.City;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.Locations
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : BaseController<CitySelectDto, CityCreateDto, CityUpdateDto, ICityService>
    {
        public CityController(ICityService service, ILogger<CityController> logger) : base(service, logger)
        {
        }

        protected override async Task AddAsync(CityCreateDto dto)
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

        protected override async Task<IEnumerable<CitySelectDto>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        protected override async Task<CitySelectDto?> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        protected override async Task<CityUpdateDto> UpdateAsync(int id, CityUpdateDto dto)
        {
            return await _service.UpdateAsync(dto);
        }
    }
}
