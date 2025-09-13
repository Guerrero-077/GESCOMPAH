using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.IDataImplement.Business;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.EstablishmentDto;
using Entity.Infrastructure.Context;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business.Services.Business
{
    /// <summary>
    /// Servicio de Establecimientos con validaciones de dominio
    /// y proyección liviana para cálculos en contratos.
    /// </summary>
    public sealed class EstablishmentService :
        BusinessGeneric<EstablishmentSelectDto, EstablishmentCreateDto, EstablishmentUpdateDto, Establishment>,
        IEstablishmentService
    {
        private readonly IEstablishmentsRepository _repo;
        private readonly ILogger<EstablishmentService> _logger;

        public EstablishmentService(
            IEstablishmentsRepository repo,
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<EstablishmentService> logger
        ) : base(repo, mapper)
        {
            _repo = repo;
            _logger = logger;
        }

        // ========= LISTAS =========

        /// <summary>Todos: activos e inactivos.</summary>
        public async Task<IReadOnlyList<EstablishmentSelectDto>> GetAllAnyAsync()
        {
            var list = await _repo.GetAllAsync(); // ActivityFilter.Any (compat)
            return list.Select(e => e.Adapt<EstablishmentSelectDto>()).ToList().AsReadOnly();
        }

        /// <summary>Solo activos.</summary>
        public async Task<IReadOnlyList<EstablishmentSelectDto>> GetAllActiveAsync()
        {
            var list = await _repo.GetAllAsync(ActivityFilter.ActiveOnly);
            return list.Select(e => e.Adapt<EstablishmentSelectDto>()).ToList().AsReadOnly();
        }

        // ========= DETALLE =========

        public async Task<EstablishmentSelectDto?> GetByIdAnyAsync(int id)
        {
            var e = await _repo.GetByIdAnyAsync(id);
            return e?.Adapt<EstablishmentSelectDto>();
        }

        public async Task<EstablishmentSelectDto?> GetByIdActiveAsync(int id)
        {
            var e = await _repo.GetByIdActiveAsync(id);
            return e?.Adapt<EstablishmentSelectDto>();
        }

        // ========= CRUD =========

        public override async Task<EstablishmentSelectDto> CreateAsync(EstablishmentCreateDto dto)
        {
            try
            {
                Validate(dto);

                var entity = dto.Adapt<Establishment>();

                await _repo.AddAsync(entity);

                // Recarga sin filtro por Active (o usa el entity si no hace falta)
                var created = await _repo.GetByIdAnyAsync(entity.Id) ?? entity;

                return created.Adapt<EstablishmentSelectDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear establecimiento");
                throw new BusinessException("Error al crear el establecimiento.", ex);
            }
        }

        public override async Task<EstablishmentSelectDto?> UpdateAsync(EstablishmentUpdateDto dto)
        {
            try
            {
                var entity = await _repo.GetByIdAnyAsync(dto.Id);
                if (entity is null) return null;

                Validate(dto);
                dto.Adapt(entity);

                await _repo.UpdateAsync(entity);

                var reloaded = await _repo.GetByIdAnyAsync(entity.Id) ?? entity;

                return reloaded.Adapt<EstablishmentSelectDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar establecimiento {Id}", dto.Id);
                throw new BusinessException("Error al actualizar el establecimiento.", ex);
            }
        }

        // ========= PROYECCIÓN =========

        public async Task<IReadOnlyList<EstablishmentBasicsDto>> GetBasicsByIdsAsync(IEnumerable<int> ids)
        {
            var distinct = ids?.Distinct().ToList() ?? new List<int>();
            if (distinct.Count == 0) return Array.Empty<EstablishmentBasicsDto>();

            var basics = await _repo.GetBasicsByIdsAsync(distinct);
            return basics.Select(b => new EstablishmentBasicsDto(b.Id, b.RentValueBase, b.UvtQty))
                         .ToList()
                         .AsReadOnly();
        }

        // Lista liviana para grid/cards (sin Includes pesados)
        public async Task<IReadOnlyList<EstablishmentCardLiteDto>> GetCardsAnyAsync()
        {
            var list = await _repo.GetCardsAsync(ActivityFilter.Any);
            return list.Select(e => new EstablishmentCardLiteDto(
                e.Id,
                e.Name,
                e.Description,
                e.Address,
                e.AreaM2,
                e.RentValueBase,
                e.Active,
                e.PrimaryImagePath
            )).ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<EstablishmentCardLiteDto>> GetCardsActiveAsync()
        {
            var list = await _repo.GetCardsAsync(ActivityFilter.ActiveOnly);
            return list.Select(e => new EstablishmentCardLiteDto(
                e.Id,
                e.Name,
                e.Description,
                e.Address,
                e.AreaM2,
                e.RentValueBase,
                e.Active,
                e.PrimaryImagePath
            )).ToList().AsReadOnly();
        }

        // ========= VALIDACIONES =========

        private static void Validate(EstablishmentCreateDto dto)
        {
            if (dto.RentValueBase <= 0) throw new BusinessException("RentValueBase debe ser mayor que 0.");
            if (dto.AreaM2 <= 0) throw new BusinessException("AreaM2 debe ser mayor que 0.");
            if (dto.UvtQty <= 0) throw new BusinessException("UvtQty debe ser mayor que 0.");
            if (dto.PlazaId <= 0) throw new BusinessException("PlazaId inválido.");
        }

        private static void Validate(EstablishmentUpdateDto dto)
        {
            if (dto.RentValueBase <= 0) throw new BusinessException("RentValueBase debe ser mayor que 0.");
            if (dto.AreaM2 <= 0) throw new BusinessException("AreaM2 debe ser mayor que 0.");
            if (dto.UvtQty <= 0) throw new BusinessException("UvtQty debe ser mayor que 0.");
            if (dto.PlazaId <= 0) throw new BusinessException("PlazaId inválido.");
        }
    }
}
