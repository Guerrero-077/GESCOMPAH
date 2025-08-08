using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;

namespace Data.Interfaz.IDataImplemenent.SecurityAuthentication
{
    public interface IUserRepository : IDataGeneric<User>
    {
        Task<bool> ExistsByEmailAsync(string email);
        Task<User?> GetByEmailAsync(string email);
        Task<User> LoginUser(LoginDto loginDto);
        Task<User?> GetByEmailProjectionAsync(string email);
    }
}
