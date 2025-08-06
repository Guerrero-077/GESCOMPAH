using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Contract;
using MapsterMapper;

namespace Business.Services.Business
{
    public class ContractService : BusinessGeneric<ContractSelectDto, ContractCreateDto, ContractUpdateDto, Contract>
    {
        public ContractService(IDataGeneric<Contract> data, IMapper mapper) : base(data, mapper)
        {
        }
    }
}
