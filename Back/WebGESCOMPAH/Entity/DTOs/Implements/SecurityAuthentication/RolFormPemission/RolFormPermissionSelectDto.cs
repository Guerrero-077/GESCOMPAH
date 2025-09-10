using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission
{
    public class RolFormPermissionSelectDto : BaseDto
    {
        public int Id { get; set; }
        public string RolName { get; set; }
        public int RolId { get; set; }
        public string FormName { get; set; }
        public int FormId { get; set; } 
        public List<PermissionInfoDto> Permissions { get; set; } = new List<PermissionInfoDto>();

        public bool Active { get; set; }
    }
}
