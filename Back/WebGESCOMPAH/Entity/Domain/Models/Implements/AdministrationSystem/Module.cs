using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.AdministrationSystem
{
    public class Module : BaseModelGeneric
    {
        public string Icon { get; set; } = null!;


        public ICollection<FormModule> FormModules { get; set; } = new List<FormModule>();
    }
}
