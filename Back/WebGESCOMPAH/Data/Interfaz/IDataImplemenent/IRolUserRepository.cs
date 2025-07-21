using Entity.Domain.Models.Implements.SecurityAuthentication;

namespace Data.Interfaz.IDataImplemenent
{
    public interface IRolUserRepository
    {
        Task<RolUser> AsignateRolDefault(User user);
        Task<List<Rol>> GetRolesByUserIdAsync(int userId);

    }
}
