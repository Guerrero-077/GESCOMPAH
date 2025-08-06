using Data.Interfaz.IDataImplemenent.SecurityAuthentication;
using Data.Repository;
using Entity.DTOs.Implements.SecurityAuthentication.Auth.RestPasword;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.SecurityAuthentication
{
    public class PasswordResetCodeRepository : DataGeneric<PasswordResetCode>, IPasswordResetCodeRepository
    {
        public PasswordResetCodeRepository(ApplicationDbContext context) : base(context) { }

        public async Task<PasswordResetCode?> GetValidCodeAsync(string email, string code)
        {
            return await _dbSet.FirstOrDefaultAsync(c =>
                c.Email == email &&
                c.Code == code &&
                !c.IsUsed &&
                c.Expiration > DateTime.UtcNow);
        }
    }
}
