using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.SecurityAuthentication.Permission;

namespace Business.Interfaces.Implements.SecrutityAuthentication
{
    public interface IPermissionService : IBusiness<PermissionSelectDto, PermissionCreateDto, PermissionUpdateDto>
    {
    }
}
