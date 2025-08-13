using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.SecurityAuthentication.Permission
{
    public class PermissionSelectDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Active { get; set; }

    }
}
