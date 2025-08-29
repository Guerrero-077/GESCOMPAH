using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;

namespace Data.Interfaz.IDataImplement.Business
{
    public sealed record EstablishmentBasics(int Id, decimal RentValueBase, decimal UvtQty);

    public interface IEstablishmentsRepository : IDataGeneric<Establishment>
    {
        Task<IReadOnlyList<EstablishmentBasics>> GetBasicsByIdsAsync(IEnumerable<int> ids);
    }
}
