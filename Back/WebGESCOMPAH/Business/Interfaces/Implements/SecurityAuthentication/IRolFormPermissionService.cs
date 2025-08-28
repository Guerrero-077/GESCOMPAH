using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission;

namespace Business.Interfaces.Implements.SecurityAuthentication
{
    public interface IRolFormPermissionService : IBusiness<RolFormPermissionSelectDto, RolFormPermissionCreateDto, RolFormPermissionUpdateDto>
    {

        Task<IEnumerable<RolFormPermissionSelectDto>> GetAllGroupedAsync();
        Task<bool> DeleteByGroupAsync(int rolId, int formId);
    }
}
