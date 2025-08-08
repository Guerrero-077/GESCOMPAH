using Business.Interfaces.Implements.SecrutityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

namespace WebGESCOMPAH.Controllers.Module.SecurityAuthentication
{
    [ApiController]
    //[Authorize]
    [Produces("application/json")]

    [Route("api/[controller]")]
    public class UserController : BaseController<UserSelectDto, UserCreateDto, UserUpdateDto, IUserService>
    {
        public UserController(IUserService service, ILogger<UserController> logger) : base(service, logger)
        {
        }

        protected override async Task AddAsync(UserCreateDto dto)
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

        protected override async Task<IEnumerable<UserSelectDto>> GetAllAsync()
        {
            return await _service.GetAllAsync();
        }

        protected override async Task<UserSelectDto?> GetByIdAsync(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        protected override async Task<UserSelectDto> UpdateAsync(int id, UserUpdateDto dto)
        {
            return await _service.UpdateAsync(dto);
        }
    }
}
