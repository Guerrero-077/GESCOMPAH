using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.SecurityAuthentication
{
    public class RolFormPermission : BaseModel
    {
        public int RolId { get; set; }
        public Rol Rol { get; set; } = null!;

        public int FormId { get; set; }
        public Form Form { get; set; } = null!;

        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;
    }
}
