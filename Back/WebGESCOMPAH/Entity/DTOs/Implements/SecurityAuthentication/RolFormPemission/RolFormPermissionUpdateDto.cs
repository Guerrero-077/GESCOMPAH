using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission
{
    public class RolFormPermissionUpdateDto : BaseDto
    {
        public int RolId { get; set; }

        public int FormId { get; set; }

        public int PermissionId { get; set; }
    }
}
