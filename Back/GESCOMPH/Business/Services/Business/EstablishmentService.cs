using Business.Interfaces.Implements.Business;
using Business.Repository;
using Business.Services.Validation;
using Data.Interfaz.IDataImplement.Business;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.EstablishmentDto;
using Entity.Enum;
using Entity.Infrastructure.Context;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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
        private readonly IDataGeneric<SystemParameter> _systemParamRepository;

        public EstablishmentService(
            IEstablishmentsRepository repo,
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<EstablishmentService> logger,
            IDataGeneric<SystemParameter> systemParamRepository
        ) : base(repo, mapper)
        {
            _repo = repo;
            _logger = logger;
            _systemParamRepository = systemParamRepository;
        }

        // Aquí defines la llave “única” de negocio
        protected override IQueryable<Establishment>? ApplyUniquenessFilter(IQueryable<Establishment> query, Establishment candidate)
            => query.Where(e => e.Name == candidate.Name);

        // ========= LISTAS =========

        /// <summary>Todos: activos e inactivos.</summary>
        public async Task<IReadOnlyList<EstablishmentSelectDto>> GetAllAnyAsync(int? limit = null)
        {
            var list = await _repo.GetAllAsync(ActivityFilter.Any, limit);
            return list.Select(e => e.Adapt<EstablishmentSelectDto>()).ToList().AsReadOnly();
        }

        /// <summary>Solo activos.</summary>
        public async Task<IReadOnlyList<EstablishmentSelectDto>> GetAllActiveAsync(int? limit = null)
        {
            var list = await _repo.GetAllAsync(ActivityFilter.ActiveOnly, limit);
            return list.Select(e => e.Adapt<EstablishmentSelectDto>()).ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<EstablishmentSelectDto>> GetByPlazaIdAsync(int plazaId, bool activeOnly = false, int? limit = null)
        {
            if (plazaId <= 0) return Array.Empty<EstablishmentSelectDto>();

            var filter = activeOnly ? ActivityFilter.ActiveOnly : ActivityFilter.Any;
            var list = await _repo.GetByPlazaIdAsync(plazaId, filter, limit);
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

                // Calcular RentValueBase a partir del parámetro UVT vigente
                var uvtValue = await GetParameterValueAsync("UVT", DateTime.UtcNow);
                entity.RentValueBase = Math.Round(dto.UvtQty * uvtValue, 2, MidpointRounding.AwayFromZero);

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

                // Recalcular RentValueBase a partir del parámetro UVT vigente (ignora valor recibido)
                var uvtValue = await GetParameterValueAsync("UVT", DateTime.UtcNow);
                entity.RentValueBase = Math.Round(entity.UvtQty * uvtValue, 2, MidpointRounding.AwayFromZero);

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
            return basics.ToList().AsReadOnly();
        }

        public async Task<(decimal totalBaseRent, decimal totalUvt)> ReserveForContractAsync(IReadOnlyCollection<int> ids)
        {
            var distinct = ids?.Where(id => id > 0).Distinct().ToList() ?? new List<int>();
            if (distinct.Count == 0)
                throw new BusinessException("Debe seleccionar al menos un establecimiento.");

            var inactive = await _repo.GetInactiveIdsAsync(distinct);
            if (inactive.Count > 0)
                throw new BusinessException($"Los establecimientos {string.Join(", ", inactive)} no estan disponibles (Active = false).");

            var basics = await _repo.GetBasicsByIdsAsync(distinct);
            if (basics.Count != distinct.Count)
                throw new BusinessException("Conflicto de concurrencia al recuperar los establecimientos seleccionados.");

            var totalBase = basics.Sum(b => b.RentValueBase);
            var totalUvt = basics.Sum(b => b.UvtQty);

            var affected = await _repo.SetActiveByIdsAsync(distinct, active: false);
            if (affected != distinct.Count)
                throw new BusinessException("Conflicto de concurrencia al actualizar estados de establecimientos.");

            return (totalBase, totalUvt);
        }

        // Lista liviana para grid/cards (sin Includes pesados)
        public async Task<IReadOnlyList<EstablishmentCardDto>> GetCardsAnyAsync()
        {
            var list = await _repo.GetCardsAsync(ActivityFilter.Any);
            return list.ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<EstablishmentCardDto>> GetCardsActiveAsync()
        {
            var list = await _repo.GetCardsAsync(ActivityFilter.ActiveOnly);
            return list.ToList().AsReadOnly();
        }

        // ========= VALIDACIONES =========

        private static void Validate(EstablishmentCreateDto dto)
        {
            if (dto is null)
                throw new BusinessException("Payload inválido.");

            NormalizeCreate(dto);
        }

        private static void Validate(EstablishmentUpdateDto dto)
        {
            if (dto is null)
                throw new BusinessException("Payload inválido.");

            DomainValidation.EnsureId(dto.Id, "EstablishmentId");
            NormalizeUpdate(dto);
        }

        private static void NormalizeCreate(EstablishmentCreateDto dto)
        {
            dto.Name = DomainValidation.RequireText(dto.Name, "Nombre", 100);
            dto.Description = DomainValidation.RequireText(dto.Description, "Descripción", 500);
            dto.UvtQty = DomainValidation.EnsureDecimalRange(dto.UvtQty, 1m, 9_999m, 2, "UvtQty");
            dto.AreaM2 = DomainValidation.EnsureDecimalRange(dto.AreaM2, 1m, 1_000_000m, 2, "AreaM2");
            dto.Address = DomainValidation.NormalizeAddress(dto.Address, required: false, maxLength: 150);
            DomainValidation.EnsureId(dto.PlazaId, "PlazaId");
        }

        private static void NormalizeUpdate(EstablishmentUpdateDto dto)
        {
            dto.Name = DomainValidation.RequireText(dto.Name, "Nombre", 100);
            dto.Description = DomainValidation.RequireText(dto.Description, "Descripción", 500);
            dto.UvtQty = DomainValidation.EnsureDecimalRange(dto.UvtQty, 1m, 9_999m, 2, "UvtQty");
            dto.AreaM2 = DomainValidation.EnsureDecimalRange(dto.AreaM2, 1m, 1_000_000m, 2, "AreaM2");
            dto.Address = DomainValidation.NormalizeAddress(dto.Address, required: false, maxLength: 150);
            DomainValidation.EnsureId(dto.PlazaId, "PlazaId");
        }





        protected override Expression<Func<Establishment, string>>[] SearchableFields() =>
        [
            e => e.Name!,
            e => e.Description!,
            e => e.Address!,
            e => e.Plaza.Name!
        ];

        protected override string[] SortableFields() =>
        [
            nameof(Establishment.Name),
            nameof(Establishment.Description),
            nameof(Establishment.RentValueBase),
            nameof(Establishment.AreaM2),
            nameof(Establishment.PlazaId),
            nameof(Establishment.Id),
            nameof(Establishment.CreatedAt),
            nameof(Establishment.Active)
        ];

        // ------------------ Helpers de parámetros del sistema ------------------
        private async Task<decimal> GetParameterValueAsync(string key, DateTime date)
        {
            var param = await _systemParamRepository.GetAllQueryable()
                .Where(p => p.Key == key && p.EffectiveFrom <= date && (p.EffectiveTo == null || p.EffectiveTo >= date))
                .OrderByDescending(p => p.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (param == null)
                throw new BusinessException($"Parámetro '{key}' no encontrado para la fecha {date:yyyy-MM-dd}.");

            if (!TryParseDecimalFlexible(param.Value, out var value))
                throw new BusinessException($"Valor inválido para parámetro '{key}': '{param.Value}'.");

            if (key.Equals("UVT", StringComparison.OrdinalIgnoreCase) && value <= 0m)
                throw new BusinessException("UVT debe ser mayor que 0.");

            return value;
        }

        private static bool TryParseDecimalFlexible(string raw, out decimal value)
        {
            raw = raw?.Trim() ?? string.Empty;
            if (decimal.TryParse(raw, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out value)) return true;
            var es = CultureInfo.GetCultureInfo("es-CO");
            if (decimal.TryParse(raw, System.Globalization.NumberStyles.Any, es, out value)) return true;
            return decimal.TryParse(raw, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out value);
        }


    }
}

