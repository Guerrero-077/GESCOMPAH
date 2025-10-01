using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission
{
    public class RolFormPermissionUpdateDto
    {
        public int Id { get; set; } 
        public int RolId { get; set; }
        public int FormId { get; set; }
        public List<int> PermissionIds { get; set; } 
    }
}
