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

    public sealed record ExpirationSweepResult(
    IReadOnlyList<int> DeactivatedContractIds,
    int ReactivatedEstablishments);


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
