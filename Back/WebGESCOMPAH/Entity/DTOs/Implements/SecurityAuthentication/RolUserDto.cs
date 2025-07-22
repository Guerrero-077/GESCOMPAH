using Entity.DTOs.Implements.AdministrationSystem;

namespace Entity.DTOs.Implements.SecurityAuthentication
{
    public class RolUserDto
    {
        public int RolId { get; set; }
        public string RolName { get; set; }
        public List<RolPermissionDto> Permissions { get; set; }

    }
}
