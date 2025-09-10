using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Business.ObligationMonth
{
    public class ObligationMonthUpdateDto : BaseDto
    {
        public int ContractId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime DueDate { get; set; }
        public decimal UvtQtyApplied { get; set; }
        public decimal UvtValueApplied { get; set; }
        public decimal VatRateApplied { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int? DaysLate { get; set; }
        public decimal? LateAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool Locked { get; set; }
    }
}
