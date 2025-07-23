using Entity.DTOs.Implements.AdministrationSystem;

namespace Entity.DTOs.Implements.SecurityAuthentication.RolUser
{
    public class RolUserDto
    {
        public int RolId { get; set; }
        public string RolName { get; set; } = null!;
        public List<RolPermissionDto> Permissions { get; set; }

    }
}
