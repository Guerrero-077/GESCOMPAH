using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class ObligationMonth : BaseModel
    {

        public int ContractId { get; set; }
        public Contract Contract { get; set; } = null!;

        public int Year { get; set; }
        public int Month { get; set; }

        public DateTime DueDate { get; set; }

        // Fotocopia de los parámetros usados este mes
        public decimal UvtQtyApplied { get; set; } // normalmente igual al pactado
        public decimal UvtValueApplied { get; set; } // desde SystemParameters
        public decimal VatRateApplied { get; set; } // desde SystemParameters

        // Cálculo congelado
        public decimal BaseAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }

        // Mora (solo si se liquida)
        public int? DaysLate { get; set; }
        public decimal? LateAmount { get; set; }

        // Estado
        public string Status { get; set; }

        public bool Locked { get; set; } = false; // Evita que se recalcule si ya fue liquidado
    }
}
