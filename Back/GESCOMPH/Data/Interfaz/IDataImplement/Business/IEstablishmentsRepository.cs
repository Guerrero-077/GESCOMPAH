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

        // Sobrecarga explicita
        Task<IEnumerable<Establishment>> GetAllAsync(ActivityFilter filter, int? limit = null);
        // Consulta por plaza
        Task<IEnumerable<Establishment>> GetByPlazaIdAsync(int plazaId, ActivityFilter filter, int? limit = null);

        // Detalles
        Task<Establishment?> GetByIdAnyAsync(int id);
        Task<Establishment?> GetByIdActiveAsync(int id);

        // Proyeccion liviana
        Task<IReadOnlyList<EstablishmentBasicsDto>> GetBasicsByIdsAsync(IReadOnlyCollection<int> ids);

        // Proyeccion para listados (tarjetas)
        Task<IReadOnlyList<EstablishmentCardDto>> GetCardsAsync(ActivityFilter filter);

        // Validacion / comandos
        Task<IReadOnlyList<int>> GetInactiveIdsAsync(IReadOnlyCollection<int> ids);
        Task<int> SetActiveByIdsAsync(IReadOnlyCollection<int> ids, bool active);

        // Actualizacion masiva por Plaza
        Task<int> SetActiveByPlazaIdAsync(int plazaId, bool active);
    }
}
