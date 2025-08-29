using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Business.EstablishmentDto;

namespace Business.Interfaces.Implements.Business
{

    /// <summary>
    /// Contrato de servicio para Establecimientos.
    /// Expone operaciones CRUD y una proyección liviana para cálculos (basics).
    /// </summary>
    public sealed record EstablishmentBasicsDto(int Id, decimal RentValueBase, decimal UvtQty);

    public interface IEstablishmentService : IBusiness<EstablishmentSelectDto, EstablishmentCreateDto, EstablishmentUpdateDto>
    {
        //Task DeleteAsync(int id, bool forceDelete);
        //Task DeleteImageAsync(int establishmentId, int imageId);

        /// <summary>
        /// Proyección liviana para sumatorias (contratos): Id, RentValueBase, UvtQty.
        /// Evita materializar entidades completas y mejora rendimiento en cálculos.
        /// </summary>
        Task<IReadOnlyList<EstablishmentBasicsDto>> GetBasicsByIdsAsync(IEnumerable<int> ids);

    }

}
