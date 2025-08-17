using Entity.Domain.Models.Implements.AdministrationSystem;

namespace Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission
{

    public class RolFormPermissionCreateDto
    {
        public int RolId { get; set; }
        public int FormId { get; set; }
        public List<int> PermissionIds { get; set; } // Changed from single Id
    }
}
