using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class Calculation : BaseModel
    {
        public enum Type {};
        public string Formula { get; set; } = null!;
        public string Description { get; set; } = null!;

    }
}
