using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class PremisesLeased : BaseModel
    {
        public int CntractId { get; set; } // Foreign key to the Contract
        public Contract Contract { get; set; } = null!; // Navigation property to the Contract
        public int EstablishmentId { get; set; } // Foreign key to the Establishment
        public Establishment Establishment { get; set; } = null!; // Navigation property to the Establishment
    }
}
