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

        public override async Task<EstablishmentSelectDto> CreateAsync(EstablishmentCreateDto dto)
        {
            Validate(dto);

            var entity = dto.Adapt<Establishment>();

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                await _repo.AddAsync(entity);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();

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

        public override async Task<EstablishmentSelectDto?> UpdateAsync(EstablishmentUpdateDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity is null) return null;

            Validate(dto);
            dto.Adapt(entity);

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                await _repo.UpdateAsync(entity);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

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

        public async Task<IReadOnlyList<EstablishmentBasicsDto>> GetBasicsByIdsAsync(IEnumerable<int> ids)
        {
            var basics = await _repo.GetBasicsByIdsAsync(ids);
            return basics.Select(b => new EstablishmentBasicsDto(b.Id, b.RentValueBase, b.UvtQty))
                         .ToList()
                         .AsReadOnly();
        }

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
