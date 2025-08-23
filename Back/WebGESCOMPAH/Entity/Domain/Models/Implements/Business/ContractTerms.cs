using Entity.Domain.Models.ModelBase;
using System.Diagnostics.Contracts;

namespace Entity.Domain.Models.Implements.Business
{
    public class ContractTerms : BaseModel
    {
        public int ContractId { get; set; }
        public Contract Contract { get; set; } = null!;

        public decimal UvtQty { get; set; }

        public bool UseSystemParameters { get; set; } = true;

        public string CalculationKey { get; set; }

    }
}
