namespace Entity.DTOs.Implements.SecurityAuthentication.RolFormPemission
{
    public class RolFormPermissionUpdateDto
    {
        public int Id { get; set; }
        public int RolId { get; set; }

        public int FormId { get; set; }

        public int PermissionId { get; set; }
    }
}
