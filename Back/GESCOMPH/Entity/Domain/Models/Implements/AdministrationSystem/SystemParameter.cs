using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.AdministrationSystem
{
    public class SystemParameter : BaseModel
    {
        public string Key { get; set; } = null!; // Ej: "UVT", "IVA_RENTA", "MORA_DAILY"
        public string Value { get; set; } = null!; // Ej: "49.79875", "0.19"

        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
