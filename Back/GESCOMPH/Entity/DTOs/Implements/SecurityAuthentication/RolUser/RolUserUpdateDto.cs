using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.SecurityAuthentication.RolUser
{
    public class RolUserUpdateDto : BaseDto
    {
        public int RolId { get; set; }
        public int UserId { get; set; }
    }
}
