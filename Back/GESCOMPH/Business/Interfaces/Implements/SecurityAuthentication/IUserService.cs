using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.SecurityAuthentication.User;

namespace Business.Interfaces.Implements.SecurityAuthentication
{
    public interface IUserService : IBusiness<UserSelectDto, UserCreateDto, UserUpdateDto>
    {
        //Task<UserCreateDto> CreateUserAsync(UserCreateDto dto);
        Task<UserSelectDto> CreateWithPersonAndRolesAsync(UserCreateDto dto);
        Task<UserSelectDto> UpdateWithPersonAndRolesAsync(UserUpdateDto dto);
        Task<(int userId, bool created, string? tempPassword)> EnsureUserForPersonAsync(int personId, string email);

    }
}
