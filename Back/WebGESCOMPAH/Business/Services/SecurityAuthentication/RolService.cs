using Business.Interfaces.Implements.SecurityAuthentication;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Rol;
using MapsterMapper;
using System.Linq.Expressions;

public class RolService(IDataGeneric<Rol> rolRepository, IMapper mapper) : BusinessGeneric<RolSelectDto, RolCreateDto, RolUpdateDto, Rol>(rolRepository, mapper), IRolService
    {
            // Aquí defines la llave “única” de negocio
        protected override IQueryable<Rol>? ApplyUniquenessFilter(IQueryable<Rol> query, Rol candidate)
            => query.Where(f => f.Name == candidate.Name);

    protected override Expression<Func<Rol, string?>>[] SearchableFields() =>
    [
        r => r.Name
    ];

    // ✔️ Ordenamiento permitido
    protected override string[] SortableFields() =>
    [
        nameof(Rol.Name),
        nameof(Rol.Id),
        nameof(Rol.CreatedAt),
        nameof(Rol.Active)
    ];

    // ✔️ Filtros personalizados por query params
    protected override IDictionary<string, Func<string, Expression<Func<Rol, bool>>>> AllowedFilters() =>
        new Dictionary<string, Func<string, Expression<Func<Rol, bool>>>>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(Rol.Name)] = value => r => r.Name == value,
            [nameof(Rol.Active)] = value => r => r.Active == bool.Parse(value)
        };

}
