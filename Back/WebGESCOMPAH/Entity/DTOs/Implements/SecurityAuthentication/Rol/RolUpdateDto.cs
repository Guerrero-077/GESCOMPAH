using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.SecurityAuthentication.Rol
{
    public class RolUpdateDto : BaseDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
