using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class Calculation : BaseModel
    {
        public int Id { get; set; }

        public string Type { get; set; } 
        public string Key { get; set; }
        public string Formula { get; set; }
        public string? Description { get; set; }

        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

    }
}
