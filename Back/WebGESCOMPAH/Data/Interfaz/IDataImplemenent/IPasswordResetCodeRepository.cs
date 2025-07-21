using Data.Interfaz.DataBasic;
using Entity.DTOs.Implements.SecurityAuthentication;

namespace Data.Interfaz.IDataImplemenent
{
    public interface IPasswordResetCodeRepository : IDataGeneric<PasswordResetCode>
    {
        Task<PasswordResetCode?> GetValidCodeAsync(string email, string code);
    }
}
