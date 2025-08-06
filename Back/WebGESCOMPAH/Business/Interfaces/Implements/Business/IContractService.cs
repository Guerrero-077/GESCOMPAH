using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Business.Contract;

namespace Business.Interfaces.Implements.Business
{
    public interface IContractService :IBusiness<ContractSelectDto, ContractCreateDto, ContractUpdateDto>
    {
    }
}
