using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.SecurityAuthentication;

namespace Data.Interfaz.IDataImplemenent.SecurityAuthentication
{
    public interface IRolUserRepository : IDataGeneric<RolUser>
    {
        Task<RolUser> AsignateRolDefault(User user);
        Task<List<Rol>> GetRolesByUserIdAsync(int userId);

        Task<IEnumerable<string>> GetRoleNamesByUserIdAsync(int userId);


        Task ReplaceUserRolesAsync(int userId, IEnumerable<int> roleIds);
    }
}
