using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.EstablishmentDto;
using Entity.Enum;

namespace Data.Interfaz.IDataImplement.Business
{
    public interface IEstablishmentsRepository : IDataGeneric<Establishment>
    {

        // Sobrecarga explícita
        Task<IEnumerable<Establishment>> GetAllAsync(ActivityFilter filter);

        // Detalles
        Task<Establishment?> GetByIdAnyAsync(int id);
        Task<Establishment?> GetByIdActiveAsync(int id);

        // Proyección liviana
        Task<IReadOnlyList<EstablishmentBasicsDto>> GetBasicsByIdsAsync(IReadOnlyCollection<int> ids);

        // Proyección para listados (tarjetas)
        Task<IReadOnlyList<EstablishmentCardDto>> GetCardsAsync(ActivityFilter filter);

        // Validación / comandos
        Task<IReadOnlyList<int>> GetInactiveIdsAsync(IReadOnlyCollection<int> ids);
        Task<int> SetActiveByIdsAsync(IReadOnlyCollection<int> ids, bool active);

        // Actualización masiva por Plaza
        Task<int> SetActiveByPlazaIdAsync(int plazaId, bool active);
    }
}
