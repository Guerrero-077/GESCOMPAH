using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Business.EstablishmentDto;

namespace Business.Interfaces.Implements.Business
{
    /// <summary>
    /// Contrato de servicio para Establecimientos.
    /// CRUD + consultas por disponibilidad y proyección liviana para cálculos.
    /// </summary>
    public sealed record EstablishmentBasicsDto(int Id, decimal RentValueBase, decimal UvtQty);
    public sealed record EstablishmentCardLiteDto(
        int Id,
        string Name,
        string Description,
        string Address,
        decimal AreaM2,
        decimal RentValueBase,
        bool Active,
        string? PrimaryImagePath
    );

    public interface IEstablishmentService
        : IBusiness<EstablishmentSelectDto, EstablishmentCreateDto, EstablishmentUpdateDto>
    {
        // === LISTAS ===

        /// <summary>Obtiene TODOS los establecimientos (activos e inactivos).</summary>
        Task<IReadOnlyList<EstablishmentSelectDto>> GetAllAnyAsync();

        /// <summary>Obtiene SOLO los establecimientos activos.</summary>
        Task<IReadOnlyList<EstablishmentSelectDto>> GetAllActiveAsync();

        // === DETALLE ===

        /// <summary>Obtiene un establecimiento por Id (activo o inactivo).</summary>
        Task<EstablishmentSelectDto?> GetByIdAnyAsync(int id);

        /// <summary>Obtiene un establecimiento activo por Id (retorna null si está inactivo o no existe).</summary>
        Task<EstablishmentSelectDto?> GetByIdActiveAsync(int id);

        // === PROYECCIÓN LIVIANA (para contratos / sumatorias) ===

        /// <summary>
        /// Proyección liviana para sumatorias: Id, RentValueBase, UvtQty.
        /// Evita materializar entidades completas y mejora rendimiento en cálculos.
        /// </summary>
        Task<IReadOnlyList<EstablishmentBasicsDto>> GetBasicsByIdsAsync(IEnumerable<int> ids);

        // === LISTA LIVIANA PARA GRID/CARDS ===
        Task<IReadOnlyList<EstablishmentCardLiteDto>> GetCardsAnyAsync();
        Task<IReadOnlyList<EstablishmentCardLiteDto>> GetCardsActiveAsync();
    }
}
