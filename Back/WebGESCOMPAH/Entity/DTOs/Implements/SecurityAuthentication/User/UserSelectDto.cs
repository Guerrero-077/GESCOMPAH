using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.SecurityAuthentication.User
{
    public class UserSelectDto : BaseDto
    {
        public string PersonName { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
    }
}
