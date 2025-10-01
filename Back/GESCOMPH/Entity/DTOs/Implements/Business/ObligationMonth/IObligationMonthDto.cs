namespace Entity.DTOs.Implements.Business.ObligationMonth
{
    public interface IObligationMonthDto
    {
        int ContractId { get; set; }
        int Year { get; set; }
        int Month { get; set; }
        DateTime DueDate { get; set; }
        decimal UvtQtyApplied { get; set; }
        decimal UvtValueApplied { get; set; }
        decimal VatRateApplied { get; set; }
        decimal BaseAmount { get; set; }
        decimal VatAmount { get; set; }
        decimal TotalAmount { get; set; }
        int? DaysLate { get; set; }
        decimal? LateAmount { get; set; }
        string Status { get; set; }
        bool Locked { get; set; }
    }
}
