using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class Contract : BaseModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string? BusinessPurpose { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;

        public ICollection<ContractTerms> ContractTerms { get; set; } = new List<ContractTerms>();
        public ICollection<PremisesLeased> PremisesLeased { get; set; } = new List<PremisesLeased>();
        public ICollection<ObligationMonth> ObligationMonths { get; set; } = new List<ObligationMonth>();

        public ICollection<ContractClause> ContractClauses { get; set; } = new List<ContractClause>();
    }

}
