using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.AdministrationSystem
{
    public class SystemParameter : BaseModelGeneric
    {
        public string Value { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

    }
}
