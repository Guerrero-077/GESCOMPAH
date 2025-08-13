using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.SecurityAuthentication.Rol
{
    public class RolSelectDto : BaseDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool Active { get; set; }
    }
}
