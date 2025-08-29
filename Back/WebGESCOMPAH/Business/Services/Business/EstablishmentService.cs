using System.Linq;
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
    /// Servicio de Establecimientos (sin manejo de archivos en Create/Update).
    /// Orquesta reglas de negocio y persistencia transaccional usando ApplicationDbContext.
    /// </summary>
    public sealed class EstablishmentService :
        BusinessGeneric<EstablishmentSelectDto, EstablishmentCreateDto, EstablishmentUpdateDto, Establishment>,
        IEstablishmentService
    {
        private readonly IEstablishmentsRepository _repo;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EstablishmentService> _logger;

        public EstablishmentService(
            IEstablishmentsRepository repo,
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<EstablishmentService> logger
        ) : base(repo, mapper)
        {
            _repo = repo;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Crea un Establecimiento (sin imágenes) en una transacción corta.
        /// - No sube ni valida imágenes aquí (flujo en dos fases).
        /// - Valida reglas mínimas antes de persistir.
        /// </summary>
        public override async Task<EstablishmentSelectDto> CreateAsync(EstablishmentCreateDto dto)
        {
            // Validaciones de negocio mínimas (evitan datos inválidos)
            if (dto.RentValueBase <= 0) throw new BusinessException("RentValueBase debe ser mayor que 0.");
            if (dto.AreaM2 <= 0) throw new BusinessException("AreaM2 debe ser mayor que 0.");
            if (dto.UvtQty <= 0) throw new BusinessException("UvtQty debe ser mayor que 0.");
            if (dto.PlazaId <= 0) throw new BusinessException("PlazaId inválido.");

            var entity = dto.Adapt<Establishment>();

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                await _repo.AddAsync(entity);
                await _context.SaveChangesAsync();   // genera Id/FKs

                await tx.CommitAsync();

                // Recargar para SelectDto consistente
                var created = await _repo.GetByIdAsync(entity.Id)
                             ?? throw new BusinessException("No se pudo recargar el establecimiento creado.");
                return created.Adapt<EstablishmentSelectDto>();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error al crear establecimiento");
                throw new BusinessException("Error al crear el establecimiento.", ex);
            }
        }

        /// <summary>
        /// Actualiza datos del Establecimiento (sin gestionar imágenes).
        /// - Aplica Mapster sobre la entidad cargada.
        /// - Persiste transaccionalmente.
        /// </summary>
        public override async Task<EstablishmentSelectDto?> UpdateAsync(EstablishmentUpdateDto dto)
        {
            // Cargar existente
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity is null) return null;

            // Validaciones de negocio rápidas
            if (dto.RentValueBase <= 0) throw new BusinessException("RentValueBase debe ser mayor que 0.");
            if (dto.AreaM2 <= 0) throw new BusinessException("AreaM2 debe ser mayor que 0.");
            if (dto.UvtQty <= 0) throw new BusinessException("UvtQty debe ser mayor que 0.");
            if (dto.PlazaId <= 0) throw new BusinessException("PlazaId inválido.");

            // Aplicar cambios (respetando config Mapster)
            dto.Adapt(entity);

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                await _repo.UpdateAsync(entity);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                // Devolver DTO consistente desde BD
                var reloaded = await _repo.GetByIdAsync(entity.Id)
                               ?? throw new BusinessException("No se pudo recargar el establecimiento actualizado.");
                return reloaded.Adapt<EstablishmentSelectDto>();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error al actualizar establecimiento {Id}", dto.Id);
                throw new BusinessException("Error al actualizar el establecimiento.", ex);
            }
        }

        /// <summary>
        /// Proyección liviana para cálculos (contratos): Id, RentValueBase, UvtQty.
        /// Evita materializar entidades completas.
        /// </summary>
        public async Task<IReadOnlyList<EstablishmentBasicsDto>> GetBasicsByIdsAsync(IEnumerable<int> ids)
        {
            if (ids is null || !ids.Any())
                return Array.Empty<EstablishmentBasicsDto>();

            var basics = await _repo.GetBasicsByIdsAsync(ids);
            if (basics.Count == 0)
                throw new BusinessException("No se encontraron establecimientos válidos para los IDs proporcionados.");

            // Mapear record del repo -> DTO del servicio
            // Puedes usar Mapster si prefieres: basics.Adapt<List<EstablishmentBasicsDto>>();
            var mapped = basics
                .Select(b => new EstablishmentBasicsDto(b.Id, b.RentValueBase, b.UvtQty))
                .ToList()
                .AsReadOnly();

            return mapped;
        }
    }
}
