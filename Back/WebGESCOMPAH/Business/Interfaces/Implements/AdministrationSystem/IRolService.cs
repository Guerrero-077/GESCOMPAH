using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;

namespace Business.Interfaces.Implements.AdministrationSystem
{
    public interface IRolService : IBusiness<RolSelectDto, RolCreateDto, RolUpdateDto>
    {
    }
}
