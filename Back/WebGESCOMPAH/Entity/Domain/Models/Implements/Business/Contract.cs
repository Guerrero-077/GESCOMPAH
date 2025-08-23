using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class Contract : BaseModel
    {

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Status { get; set; } 

        public string? Destination { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;

        public ICollection<ContractTerms> ContractTerms { get; set; } = [];
        public ICollection<PremisesLeased> PremisesLeased { get; set; } = [];
        public ICollection<ObligationMonth> ObligationMonths { get; set; } = [];
    }
}
