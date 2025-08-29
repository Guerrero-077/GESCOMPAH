using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class Contract : BaseModel
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public string? BusinessPurpose { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;

        //public ICollection<ContractTerms> ContractTerms { get; set; } = new List<ContractTerms>();
        public ICollection<PremisesLeased> PremisesLeased { get; set; } = new List<PremisesLeased>();
        public ICollection<ObligationMonth> ObligationMonths { get; set; } = new List<ObligationMonth>();

        public ICollection<ContractClause> ContractClauses { get; set; } = new List<ContractClause>();


        // 🔹 NUEVO: snapshot global inmutable pactado al crear
        public decimal TotalBaseRentAgreed { get; set; }   // suma de BaseRent de todos los locales
        public decimal TotalUvtQtyAgreed { get; set; }     // suma de UvtQty de todos los locales
    }

}
