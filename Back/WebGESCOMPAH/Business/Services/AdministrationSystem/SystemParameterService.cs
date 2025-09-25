using Business.Interfaces.Implements.AdministrationSystem;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.SystemParameter;
using MapsterMapper;
using System.Linq.Expressions;
using Utilities.Exceptions;

namespace Business.Services.AdministrationSystem
{
    public class SystemParameterService : BusinessGeneric<SystemParameterSelectDto, SystemParameterDto, SystemParameterUpdateDto, SystemParameter>, ISystemParameterService
    {
        public SystemParameterService(IDataGeneric<SystemParameter> data, IMapper mapper) : base(data, mapper)
        {
        }
        protected override Expression<Func<SystemParameter, string>>[] SearchableFields() =>
        [
            sp => sp.Key!,
            sp => sp.Value!
        ];

        protected override string[] SortableFields() => new[]
        {
            nameof(SystemParameter.Key),
            nameof(SystemParameter.Value),
            nameof(SystemParameter.EffectiveFrom),
            nameof(SystemParameter.EffectiveTo),
            nameof(SystemParameter.Active),
            nameof(SystemParameter.Id),
            nameof(SystemParameter.CreatedAt)
        };

        protected override IDictionary<string, Func<string, Expression<Func<SystemParameter, bool>>>> AllowedFilters() =>
            new Dictionary<string, Func<string, Expression<Func<SystemParameter, bool>>>>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(SystemParameter.Key)] = v => e => e.Key == v,
                [nameof(SystemParameter.Active)] = v => e => e.Active == bool.Parse(v)
            };

        // Evita duplicados por (Key, EffectiveFrom)
        protected override IQueryable<SystemParameter>? ApplyUniquenessFilter(IQueryable<SystemParameter> query, SystemParameter candidate)
            => query.Where(sp => sp.Key == candidate.Key && sp.EffectiveFrom.Date == candidate.EffectiveFrom.Date);

        private static void ValidateDates(ISystemParameterDto dto)
        {
            // Validación: fecha fin no puede ser menor a fecha inicio
            if (dto is null) throw new ArgumentNullException(nameof(dto));
            var from = dto.EffectiveFrom;
            var to = dto.EffectiveTo;
            if (to.HasValue && to.Value.Date < from.Date)
                throw new BusinessException("La fecha 'Vigente hasta' no puede ser menor que 'Vigente desde'.");
        }

        public override async Task<SystemParameterSelectDto> CreateAsync(SystemParameterDto dto)
        {
            ValidateDates(dto);
            // Normalizar clave/valor
            dto.Key = dto.Key?.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Key)) dto.Key = dto.Key.ToUpperInvariant();
            dto.Value = dto.Value?.Trim();
            return await base.CreateAsync(dto);
        }

        public override async Task<SystemParameterSelectDto?> UpdateAsync(SystemParameterUpdateDto dto)
        {
            ValidateDates(dto);
            // Normalizar clave/valor
            dto.Key = dto.Key?.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Key)) dto.Key = dto.Key.ToUpperInvariant();
            dto.Value = dto.Value?.Trim();
            return await base.UpdateAsync(dto);
        }
    }
}
