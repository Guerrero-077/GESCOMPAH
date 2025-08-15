using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class ObligationMonths : BaseModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public Double BaseValue { get; set; } 
        public Double IVA { get; set; } 
        public int DaysMora { get; set; }
        public Double TotalValue { get; set; } 
        public bool PaymentNotifiedTenant { get; set; }
        public bool ValidatedAdmin { get; set; } 
        public string Description { get; set; } = null!; 
    }
}
