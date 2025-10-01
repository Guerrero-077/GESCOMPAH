using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.SecurityAuthentication;

namespace Data.Interfaz.IDataImplement.SecurityAuthentication
{
    public interface IUserRepository : IDataGeneric<User>
    {
        // Existencia
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByEmailExcludingIdAsync(int id, string email);

        // Búsquedas típicas
        Task<int?> GetIdByEmailAsync(string email);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByEmailProjectionAsync(string email);

        Task<User?> GetByIdForUpdateAsync(int id);

        // Lecturas “ricas” (detalladas) para SELECTs
        Task<User?> GetByIdWithDetailsAsync(int id);

        // Lectura mínima para autenticación (sin lógica de password aquí)
        Task<User?> GetAuthUserByEmailAsync(string email);

        // Opcionales según tu uso actual
        Task<User?> GetByPersonIdAsync(int personId);
    }
}
