using Business.Interfaces.Implements.SecrutityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.RolUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.SecurityAuthentication
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RolUserController : BaseController<RolUserSelectDto, RolUserCreateDto, RolUserUpdateDto, IRolUserService>
    {
        public RolUserController(IRolUserService service, ILogger<RolUserController> logger) : base(service, logger)
        {
        }

        protected override async Task AddAsync(RolUserCreateDto dto)
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

        protected override async Task<IEnumerable<RolUserSelectDto>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        protected override async Task<RolUserSelectDto?> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        protected override async Task<RolUserSelectDto> UpdateAsync(int id, RolUserUpdateDto dto)
        {
            return await _service.UpdateAsync(dto);
        }
    }
}
