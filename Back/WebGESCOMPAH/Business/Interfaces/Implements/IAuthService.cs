using Entity.DTOs.Implements.SecurityAuthentication;

namespace Business.Interfaces.Implements
{
    public interface IAuthService
    {
        Task<UserDto> RegisterAsync(RegisterDto dto);
        Task RequestPasswordResetAsync(string email);   
        Task ResetPasswordAsync(ConfirmResetDto dto);
    }
}
