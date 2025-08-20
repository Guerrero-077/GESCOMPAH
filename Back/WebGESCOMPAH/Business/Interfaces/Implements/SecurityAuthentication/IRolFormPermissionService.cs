using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission;

namespace Business.Interfaces.Implements.SecrutityAuthentication
{
    public interface IRolFormPermissionService : IBusiness<RolFormPermissionSelectDto, RolFormPermissionCreateDto, RolFormPermissionUpdateDto>
    {

        Task<IEnumerable<RolFormPermissionGroupedDto>> GetAllGroupedAsync();
        Task<bool> DeleteByGroupAsync(int rolId, int formId);
    }
}
