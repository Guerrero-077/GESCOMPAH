using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.Implements.SecurityAuthentication;

namespace Data.Interfaz.Security
{
    public interface IUserMeRepository
    {
        Task<User?> GetUserWithPersonAsync(int userId);
        Task<IEnumerable<RolUser>> GetUserRolesWithPermissionsAsync(int userId);
        Task<IEnumerable<Form>> GetFormsWithModulesByIdsAsync(List<int> formIds);

    }
}
