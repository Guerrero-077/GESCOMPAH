using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.SecurityAuthentication.RolUser;

namespace Business.Interfaces.Implements.SecrutityAuthentication
{
    public interface IRolUserService : IBusiness<RolUserSelectDto, RolUserCreateDto, RolUserUpdateDto>
    {
    }
}
