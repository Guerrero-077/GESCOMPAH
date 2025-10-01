using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Contract;

namespace Data.Interfaz.IDataImplement.Business
{
    public interface IContractRepository : IDataGeneric<Contract>
    {
        // Proyecciones para tarjetas de contratos
        Task<IReadOnlyList<ContractCardDto>> GetCardsByPersonAsync(int personId);
        Task<IReadOnlyList<ContractCardDto>> GetCardsAllAsync();

        // Operaciones por expiración
        Task<IReadOnlyList<int>> DeactivateExpiredAsync(DateTime utcNow);
        Task<int> ReleaseEstablishmentsForExpiredAsync(DateTime utcNow);

        // Validación de negocio: ¿existen contratos activos asociados a una plaza?
        Task<bool> AnyActiveByPlazaAsync(int plazaId);
    }
}

