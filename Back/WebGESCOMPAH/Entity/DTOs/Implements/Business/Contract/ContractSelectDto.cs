using Entity.DTOs.Base;
using Entity.DTOs.Implements.Business.ContractTerms;
using Entity.DTOs.Implements.Business.PremisesLeased;
using Entity.DTOs.Implements.Business.Clause;

namespace Entity.DTOs.Implements.Business.Contract
{
    public class ContractSelectDto : BaseDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Active { get; set; }

        // Persona asociada
        public int PersonId { get; set; }
        public string FullName { get; set; } = null!;
        public string Document { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }

        // Totales calculados (faltaban)
        public decimal TotalBaseRentAgreed { get; set; }
        public decimal TotalUvtQtyAgreed { get; set; }

        // Locales asociados
        public List<PremisesLeasedSelectDto> PremisesLeased { get; set; } = new();

        // Términos del contrato
        public ContractTermSelectDto Terms { get; set; } = null!;

        /// <summary>Cláusulas asociadas al contrato</summary>
        public List<ClauseSelectDto> Clauses { get; set; } = new();
    }
}
