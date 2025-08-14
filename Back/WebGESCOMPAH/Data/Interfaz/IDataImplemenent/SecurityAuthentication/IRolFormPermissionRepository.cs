using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.SecurityAuthentication;

namespace Data.Interfaz.IDataImplemenent.SecurityAuthentication
{
    public interface IRolFormPermissionRepository : IDataGeneric<RolFormPermission>
    {
        Task<List<int>> GetUserIdsByRoleIdAsync(int roleId);

    }
}
