using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Business.ContractTerms
{
    public class ContractTermSelectDto : BaseDto
    {
        public decimal UvtQty { get; set; }
        public string CalculationKey { get; set; } = null!;
        public bool UseSystemParameters { get; set; }
    }
}
