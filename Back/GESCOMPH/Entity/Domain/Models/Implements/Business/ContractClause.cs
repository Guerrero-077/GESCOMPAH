using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Business
{
    public class ContractClause : BaseModel
    {
        public int ContractId { get; set; }
        public Contract Contract { get; set; } = null!;

        public int ClauseId { get; set; }
        public Clause Clause { get; set; } = null!;
    }
}
