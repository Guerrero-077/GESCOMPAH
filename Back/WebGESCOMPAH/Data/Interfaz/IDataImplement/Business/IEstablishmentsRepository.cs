using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;

namespace Data.Interfaz.IDataImplement.Business
{
    public enum ActivityFilter { Any, ActiveOnly }

    public sealed record EstablishmentBasics(int Id, decimal RentValueBase, decimal UvtQty);
    public sealed record EstablishmentCard(
        int Id,
        string Name,
        string Description,
        string Address,
        decimal AreaM2,
        decimal RentValueBase,
        bool Active,
        string? PrimaryImagePath
    );

    public interface IEstablishmentsRepository : IDataGeneric<Establishment>
    {

        // Sobrecarga explícita
        Task<IEnumerable<Establishment>> GetAllAsync(ActivityFilter filter);

        // Detalles
        Task<Establishment?> GetByIdAnyAsync(int id);
        Task<Establishment?> GetByIdActiveAsync(int id);

        // Proyección liviana
        Task<IReadOnlyList<EstablishmentBasics>> GetBasicsByIdsAsync(IReadOnlyCollection<int> ids);

        // Proyección para listados (tarjetas)
        Task<IReadOnlyList<EstablishmentCard>> GetCardsAsync(ActivityFilter filter);

        // Validación / comandos
        Task<IReadOnlyList<int>> GetInactiveIdsAsync(IReadOnlyCollection<int> ids);
        Task<int> SetActiveByIdsAsync(IReadOnlyCollection<int> ids, bool active);
    }
}
