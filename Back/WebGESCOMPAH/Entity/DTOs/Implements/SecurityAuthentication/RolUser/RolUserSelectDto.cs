using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.SecurityAuthentication.RolUser
{
    public class RolUserSelectDto : BaseDto
    {
        public string RolName{ get; set; }
        public int RolId { get; set; }
        public string UserEmail { get; set; }
        public int UserId { get; set; }
        public bool Active { get; set; }
    }
}
