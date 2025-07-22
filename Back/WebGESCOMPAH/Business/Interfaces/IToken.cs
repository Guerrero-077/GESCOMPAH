using Entity.DTOs.Implements.SecurityAuthentication.Auth;

namespace Business.Interfaces
{
    public interface IToken
    {
        Task<string> GenerateToken(LoginDto dto);
    }
}
