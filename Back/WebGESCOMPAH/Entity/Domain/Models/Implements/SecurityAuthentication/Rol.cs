using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.SecurityAuthentication
{
    public class Rol : BaseModelGeneric
    {

        // Relación con RolUser
        public ICollection<RolUser> RolUsers { get; set; } = new List<RolUser>();
        public ICollection<RolFormPermission> RolFormPermissions { get; set; } = new List<RolFormPermission>();
    }
}
