using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaz.IDataImplement.Business
{
    public sealed record ContractCard(
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
        bool Active
    );

    public interface IContractRepository : IDataGeneric<Contract>
    {
        Task<IReadOnlyList<ContractCard>> GetCardsByPersonAsync(int personId);
        Task<IReadOnlyList<ContractCard>> GetCardsAllAsync();


        // NUEVOS
        Task<IReadOnlyList<int>> DeactivateExpiredAsync(DateTime utcNow);
        Task<int> ReleaseEstablishmentsForExpiredAsync(DateTime utcNow);
    }
}
