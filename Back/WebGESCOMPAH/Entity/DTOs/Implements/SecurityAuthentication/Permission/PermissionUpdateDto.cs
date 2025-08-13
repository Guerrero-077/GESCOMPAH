using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.SecurityAuthentication.Permission
{
    public class PermissionUpdateDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
