using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission
{
    public class RolFormPermissionUpdateDto
    {
        public int Id { get; set; } // This will be the ID of one of the permissions in the group, now used to identify the role/form pair
        public int RolId { get; set; }
        public int FormId { get; set; }
        public List<int> PermissionIds { get; set; } // Changed from single Id
    }
}
