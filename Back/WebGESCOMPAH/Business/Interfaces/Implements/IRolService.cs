using Entity.DTOs.Implements.SecurityAuthentication.Rol;

namespace Business.Interfaces.Implements
{
    public interface IRolService
    {
        Task<int> CreateRolAsync(RolDto rolDto);
    }
}
