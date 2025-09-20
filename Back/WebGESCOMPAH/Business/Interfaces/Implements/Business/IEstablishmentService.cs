using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Business.EstablishmentDto;

namespace Business.Interfaces.Implements.Business
{
    /// <summary>
    /// Contrato de servicio para Establecimientos.
    /// CRUD + consultas por disponibilidad y proyeccion liviana para calculos.
    /// </summary>

    public interface IEstablishmentService
        : IBusiness<EstablishmentSelectDto, EstablishmentCreateDto, EstablishmentUpdateDto>
    {
        // === LISTAS ===

        /// <summary>Obtiene TODOS los establecimientos (activos e inactivos).</summary>
        Task<IReadOnlyList<EstablishmentSelectDto>> GetAllAnyAsync(int? limit = null);

        /// <summary>Obtiene SOLO los establecimientos activos.</summary>
        Task<IReadOnlyList<EstablishmentSelectDto>> GetAllActiveAsync(int? limit = null);

        Task<IReadOnlyList<EstablishmentSelectDto>> GetByPlazaIdAsync(int plazaId, bool activeOnly = false, int? limit = null);

        // === DETALLE ===

        /// <summary>Obtiene un establecimiento por Id (activo o inactivo).</summary>
        Task<EstablishmentSelectDto?> GetByIdAnyAsync(int id);

        /// <summary>Obtiene un establecimiento activo por Id (retorna null si esta inactivo o no existe).</summary>
        Task<EstablishmentSelectDto?> GetByIdActiveAsync(int id);

        // === PROYECCION LIVIANA (para contratos / sumatorias) ===

        /// <summary>
        /// Proyeccion liviana para sumatorias: Id, RentValueBase, UvtQty.
        /// Evita materializar entidades completas y mejora rendimiento en calculos.
        /// </summary>
        Task<IReadOnlyList<EstablishmentBasicsDto>> GetBasicsByIdsAsync(IEnumerable<int> ids);

        /// <summary>
        /// Verifica disponibilidad y reserva los establecimientos para un contrato.
        /// Devuelve los totales requeridos para el calculo del contrato.
        /// </summary>
        Task<(decimal totalBaseRent, decimal totalUvt)> ReserveForContractAsync(IReadOnlyCollection<int> ids);

        // === LISTA LIVIANA PARA GRID/CARDS ===
        Task<IReadOnlyList<EstablishmentCardDto>> GetCardsAnyAsync();
        Task<IReadOnlyList<EstablishmentCardDto>> GetCardsActiveAsync();
    }
}
