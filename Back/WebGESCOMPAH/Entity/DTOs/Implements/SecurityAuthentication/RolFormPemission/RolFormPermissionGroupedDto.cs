namespace Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission
{
    public class RolFormPermissionGroupedDto
    {
        public int RolId { get; set; }
        public string RolName { get; set; }
        public int FormId { get; set; }
        public string FormName { get; set; }
        public List<PermissionInfoDto> Permissions { get; set; }
        public bool Active { get; set; }
    }
}
