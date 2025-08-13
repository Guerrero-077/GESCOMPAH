using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.SecurityAuthentication.User
{
    public class UserUpdateDto : BaseDto
    {
        public string Email { get; set; } = null!;
        public string? Password { get; set; }
    }
}
