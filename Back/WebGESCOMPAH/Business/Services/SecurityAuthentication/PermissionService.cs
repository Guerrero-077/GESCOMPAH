using Business.Interfaces.Implements.SecurityAuthentication;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Permission;
using MapsterMapper;
using System.Linq.Expressions;

namespace Business.Services.SecurityAuthentication
{
    public class PermissionService(IDataGeneric<Permission> data, IMapper mapper)
        : BusinessGeneric<PermissionSelectDto, PermissionCreateDto, PermissionUpdateDto, Permission>(data, mapper),
          IPermissionService
    {
        // Aquí defines la llave “única” de negocio
        protected override IQueryable<Permission>? ApplyUniquenessFilter(IQueryable<Permission> query, Permission candidate)
            => query.Where(p => p.Name == candidate.Name);

        protected override Expression<Func<Permission, string?>>[] SearchableFields() =>
        [
            p => p.Name,
            p => p.Description
        ];

        protected override string[] SortableFields() =>
        [
            nameof(Permission.Name),
            nameof(Permission.Description),
            nameof(Permission.Id),
            nameof(Permission.CreatedAt),
            nameof(Permission.Active)
        ];

        protected override IDictionary<string, Func<string, Expression<Func<Permission, bool>>>> AllowedFilters() =>
            new Dictionary<string, Func<string, Expression<Func<Permission, bool>>>>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(Permission.Name)] = value => p => p.Name == value,
                [nameof(Permission.Active)] = value => p => p.Active == bool.Parse(value)
            };
    }
}
