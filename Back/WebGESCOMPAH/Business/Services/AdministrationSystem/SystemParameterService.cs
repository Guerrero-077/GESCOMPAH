using Business.Interfaces.Implements.AdministrationSystem;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.DTOs.Implements.AdministrationSystem.SystemParameter;
using MapsterMapper;
using System.Linq.Expressions;

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

    }
}
