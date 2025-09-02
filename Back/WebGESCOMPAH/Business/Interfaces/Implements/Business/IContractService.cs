using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Business.Contract;

namespace Business.Interfaces.Implements.Business
{

    public sealed record ContractCardDto(
        int Id,
        int PersonId,
        string PersonFullName,
        string PersonDocument,
        string PersonPhone,
        string? PersonEmail,
        DateTime StartDate,
        DateTime EndDate,
        decimal TotalBase,
        decimal TotalUvt,
        bool Active);

    public interface IContractService : IBusiness<ContractSelectDto, ContractCreateDto, ContractUpdateDto>
    {
        Task<int> CreateContractWithPersonHandlingAsync(ContractCreateDto dto);
        Task<IReadOnlyList<ContractCardDto>> GetMineAsync();
    }
}
