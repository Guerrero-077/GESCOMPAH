using Entity.DTOs.Implements.SecurityAuthentication.Me;

namespace Business.Interfaces.Implements.SecrutityAuthentication
{
    public interface IUserContextService
    {
        Task<UserMeDto> BuildUserContextAsync(int userId);
    }
}
