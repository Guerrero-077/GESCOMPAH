using Business.Interfaces.Implements.SecrutityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Controllers.Base;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]


public class RolController : BaseController<RolSelectDto, RolCreateDto, RolUpdateDto, IRolService>
{
    public RolController(IRolService service, ILogger<RolController> logger) : base(service, logger)
    {
    }

    protected override async Task AddAsync(RolCreateDto dto)
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

    protected override async Task<IEnumerable<RolSelectDto>> GetAllAsync()
    {
        return await _service.GetAllAsync();
    }

    protected override async Task<RolSelectDto?> GetByIdAsync(int id)
    {
        return await _service.GetByIdAsync(id);
    }

    protected override async Task<RolSelectDto> UpdateAsync(int id, RolUpdateDto dto)
    {
        return await _service.UpdateAsync(dto);
    }
}
