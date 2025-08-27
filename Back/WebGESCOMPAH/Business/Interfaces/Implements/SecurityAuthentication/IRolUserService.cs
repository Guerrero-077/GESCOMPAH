using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.SecurityAuthentication.RolUser;

namespace Business.Interfaces.Implements.SecurityAuthentication
{
    public interface IRolUserService : IBusiness<RolUserSelectDto, RolUserCreateDto, RolUserUpdateDto>
    {
    }
}
