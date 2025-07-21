using Entity.DTOs.Implements.SecurityAuthentication;

namespace Business.Interfaces
{
    public interface IToken
    {
        Task<string> GenerateToken(LoginDto dto);
    }
}
