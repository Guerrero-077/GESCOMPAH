using Business.Interfaces.Implements.SecurityAuthentication;
using Business.Repository;
using Data.Interfaz.IDataImplement.SecurityAuthentication;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.RolUser;
using MapsterMapper;
using System.Linq.Expressions;

namespace Business.Services.SecurityAuthentication
{
    public class RolUserService : BusinessGeneric<RolUserSelectDto, RolUserCreateDto, RolUserUpdateDto, RolUser>, IRolUserService
    {
        private readonly IRolUserRepository _repository;

        public RolUserService(IRolUserRepository data, IMapper mapper)
            : base(data, mapper)
        {
            _repository = data;
        }

        public Task ReplaceUserRolesAsync(int userId, IEnumerable<int> roleIds)
            => _repository.ReplaceUserRolesAsync(userId, roleIds);

        // ✅ NUEVO: Soporte para búsqueda dinámica (name, email)
        protected override Expression<Func<RolUser, string?>>[] SearchableFields() =>
        [
            x => x.Rol.Name,
            x => x.User.Email
        ];

        // ✅ NUEVO: Soporte para ordenamiento
        protected override string[] SortableFields() =>
        [
            nameof(RolUser.RolId),
            nameof(RolUser.UserId),
            nameof(RolUser.Id),
            nameof(RolUser.CreatedAt),
            nameof(RolUser.Active)
        ];

        // ✅ NUEVO: Soporte para filtros por query params
        protected override IDictionary<string, Func<string, Expression<Func<RolUser, bool>>>> AllowedFilters() =>
            new Dictionary<string, Func<string, Expression<Func<RolUser, bool>>>>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(RolUser.RolId)] = value => x => x.RolId == int.Parse(value),
                [nameof(RolUser.UserId)] = value => x => x.UserId == int.Parse(value),
                [nameof(RolUser.Active)] = value => x => x.Active == bool.Parse(value)
            };
    }
}
