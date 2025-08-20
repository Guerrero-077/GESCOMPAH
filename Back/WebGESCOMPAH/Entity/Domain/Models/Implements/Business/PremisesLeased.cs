using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class PremisesLeased : BaseModel
    {
        public int ContractId { get; set; }
        public Contract Contract { get; set; } = null!;
        public int EstablishmentId { get; set; }
        public Establishment Establishment { get; set; } = null!;
    }
}
