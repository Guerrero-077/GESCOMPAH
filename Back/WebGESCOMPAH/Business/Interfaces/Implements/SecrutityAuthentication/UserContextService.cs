using Data.Interfaz.Security;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Me;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Caching.Memory;
using Utilities.Exceptions;

namespace Business.Interfaces.Implements.SecrutityAuthentication
{
    public class UserContextService : IUserContextService
    {
        private readonly IUserMeRepository _repo;
        private readonly IMapper _mapper; // Mapster IMapper
        private readonly IMemoryCache _cache; // opcional
        private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(10);

        public UserContextService(IUserMeRepository repo, IMapper mapper, IMemoryCache cache)
        {
            _repo = repo;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<UserMeDto> BuildUserContextAsync(int userId)
        {
            var cacheKey = $"UserContext:{userId}";
            if (_cache.TryGetValue<UserMeDto>(cacheKey, out var cached))
                return cached;

            var user = await _repo.GetUserWithFullContextAsync(userId)
                       ?? throw new BusinessException("Usuario no encontrado");

            // Roles: solo activos/no eliminados
            var roles = user.RolUsers?
                .Select(ru => ru.Rol)
                .Where(r => r != null && r.Active && !r.IsDeleted)
                .DistinctBy(r => r.Id)
                .ToList() ?? new List<Rol>();

            var roleNames = roles.Select(r => r.Name!).ToList();

            // Permisos: normalizados
            var permissions = roles
                .SelectMany(r => r.RolFormPermissions ?? Enumerable.Empty<RolFormPermission>())
                .Where(rfp => rfp.Permission != null && !string.IsNullOrWhiteSpace(rfp.Permission.Name))
                .Select(rfp => NormalizePermission(rfp.Permission.Name!))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Formularios autorizados (filtrando activos)
            var allowedForms = roles
                .SelectMany(r => r.RolFormPermissions ?? Enumerable.Empty<RolFormPermission>())
                .Select(rfp => rfp.Form)
                .Where(f => f != null && f.Active && !f.IsDeleted)
                .DistinctBy(f => f.Id)
                .ToList();

            // Módulos: agrupar forms por module (filtrando módulos activos)
            var modules = allowedForms
                .SelectMany(f => f.FormModules ?? Enumerable.Empty<FormModule>())
                .Select(fm => fm.Module)
                .Where(m => m != null && m.Active && !m.IsDeleted)
                .DistinctBy(m => m.Id)
                .OrderBy(m => m.Name)
                .Select(module =>
                {
                    var moduleDto = module.Adapt<MenuModuleDto>(); // mapster
                    var moduleForms = allowedForms
                        .Where(f => f.FormModules.Any(fm => fm.ModuleId == module.Id))
                        .OrderBy(f => f.Name)
                        .Select(f =>
                        {
                            var formDto = f.Adapt<FormDto>();
                            // permissions for this form:
                            var formPerms = roles
                                .SelectMany(r => r.RolFormPermissions ?? Enumerable.Empty<RolFormPermission>())
                                .Where(rfp => rfp.FormId == f.Id && rfp.Permission != null)
                                .Select(rfp => NormalizePermission(rfp.Permission.Name!))
                                .Distinct()
                                .ToList();

                            formDto.Permissions = formPerms;
                            return formDto;
                        })
                        .ToList();

                    moduleDto.Forms = moduleForms;
                    return moduleDto;
                })
                .ToList();

            var dto = new UserMeDto
            {
                Id = user.Id,
                FullName = $"{user.Person?.FirstName} {user.Person?.LastName}".Trim(),
                Email = user.Email,
                Roles = roleNames,
                Permissions = permissions,
                Menu = modules
            };

            // Cachear (opcional)
            _cache.Set(cacheKey, dto, _cacheTtl);

            return dto;
        }

        private static string NormalizePermission(string p)
            => p.Trim().ToUpperInvariant().Replace(" ", "_"); // ejemplo simple

    }

}
