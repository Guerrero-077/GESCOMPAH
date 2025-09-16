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
        public decimal UvtQtyApplied { get; set; }
        public decimal UvtValueApplied { get; set; }
        public decimal VatRateApplied { get; set; }

        // Cálculo congelado
        public decimal BaseAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }

        // Mora
        public int? DaysLate { get; set; }
        public decimal? LateAmount { get; set; }

        // Estado
        public string Status { get; set; }

        public bool Locked { get; set; } = false;
    }
}
