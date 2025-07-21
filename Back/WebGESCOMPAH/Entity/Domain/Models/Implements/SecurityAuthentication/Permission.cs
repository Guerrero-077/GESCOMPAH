using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.SecurityAuthentication
{
    public class Permission :  BaseModelGeneric
    {

        // Relación con RolFormPermission
        public virtual ICollection<RolFormPermission> RolFormPermissions { get; set; } = new List<RolFormPermission>();
    }
}
