using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Entity.DTOs.Implements.SecurityAuthentication.Auth.RestPasword;
using Entity.DTOs.Implements.SecurityAuthentication.Me;
using Entity.DTOs.Implements.SecurityAuthentication.User;

namespace Business.Interfaces.Implements.SecurityAuthentication
{
    public interface IAuthService
    {

        //Task<UserDto> RegisterAsync(RegisterDto dto);
        Task RequestPasswordResetAsync(string email);   
        Task<UserMeDto> BuildUserContextAsync(int userId);
        Task ResetPasswordAsync(ConfirmResetDto dto);
        Task ChangePasswordAsync(ChangePasswordDto dto);
    }
}
