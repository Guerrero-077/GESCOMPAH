using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Business.Contract;

namespace Business.Interfaces.Implements.Business
{
    public interface IContractService : IBusiness<ContractSelectDto, ContractCreateDto, ContractUpdateDto>
    {
        Task<int> CreateContractWithPersonHandlingAsync(ContractCreateDto dto);
        Task<IReadOnlyList<ContractCardDto>> GetMineAsync();

        // NUEVO: barrido de expiración (solo maneja Active)
        Task<ExpirationSweepResult> RunExpirationSweepAsync(CancellationToken ct = default);

        // Obligaciones del contrato
        Task<IReadOnlyList<Entity.DTOs.Implements.Business.ObligationMonth.ObligationMonthSelectDto>>
            GetObligationsAsync(int contractId);
    }
}
