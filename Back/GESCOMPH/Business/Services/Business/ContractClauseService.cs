using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.ContractClause;
using MapsterMapper;

namespace Business.Services.Business
{
    public class ContractClauseService : BusinessGeneric<ContractClauseSelectDto, ContractClauseDto, ContractClauseUpdateDto, ContractClause>, IContractClauseService
    {
        public ContractClauseService(IDataGeneric<ContractClause> data, IMapper mapper) : base(data, mapper)
        {
        }
    }
}
