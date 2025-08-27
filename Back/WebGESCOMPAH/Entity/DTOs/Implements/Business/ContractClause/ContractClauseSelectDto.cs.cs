using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.Business.ContractClause
{
    public class ContractClauseSelectDto : BaseDto
    {

        public int ClauseId { get; set; }
        public string ClauseName { get; set; }
        public string ClaseDescription { get; set; }

        public int ContractId { get; set; }
    }
}
