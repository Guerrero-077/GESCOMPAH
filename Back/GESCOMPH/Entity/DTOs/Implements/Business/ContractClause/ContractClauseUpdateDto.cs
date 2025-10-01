using Entity.DTOs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.Implements.Business.ContractClause
{
    public class ContractClauseUpdateDto : BaseDto
    {

        public int ContractId { get; set; }
        public int ClauseId { get; set; }
    }
}
