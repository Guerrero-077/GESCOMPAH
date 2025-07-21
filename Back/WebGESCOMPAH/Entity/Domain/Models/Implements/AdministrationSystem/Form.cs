using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.AdministrationSystem
{
    public class Form : BaseModelGeneric
    {
        public string Route { get; set; } = null!;

        public ICollection<RolFormPermission> RolFormPermissions { get; set; } = new List<RolFormPermission>();
        public ICollection<FormModule> FormModules { get; set; } = new List<FormModule>();
    }
}
