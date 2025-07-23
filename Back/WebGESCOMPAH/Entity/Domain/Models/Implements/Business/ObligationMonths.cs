using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class ObligationMonths : BaseModel
    {
        public int Year { get; set; } // Year of the obligation
        public int Month { get; set; } // 1-12 for January to December
        public Double BaseValue { get; set; } // Base value of the obligation   
        public Double IVA { get; set; } // IVA (Value Added Tax) applicable to the obligation
        public int DaysMoara { get; set; } // Number of days in the month for the obligation
        public Double TotalValue { get; set; } // Total value of the obligation after applying IVA and other calculations
        public bool PaymentNotifiedTenant { get; set; } // Indicates if the payment has been notified to the tenant
        public bool ValidatedAdmin { get; set; } // Indicates if the obligation has been validated
        public string Description { get; set; } = null!; // Description of the obligation
    }
}
