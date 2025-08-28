using Entity.DTOs.Implements.SecurityAuthentication.Me;

namespace Business.Interfaces.Implements.SecurityAuthentication
{
    public interface IUserContextService
    {
        Task<UserMeDto> BuildUserContextAsync(int userId);
        void InvalidateCache(int userId);
    }
}
