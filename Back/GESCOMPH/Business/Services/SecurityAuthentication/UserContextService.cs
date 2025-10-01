﻿using Business.Interfaces.Implements.SecurityAuthentication;
using Data.Interfaz.Security;
using Entity.Domain.Models.Implements.AdministrationSystem;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Me;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Caching.Memory;
using Utilities.Exceptions;

namespace Business.Services.SecurityAuthentication
{
    public class UserContextService : IUserContextService
    {
        private readonly IUserMeRepository _repo;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(10);

        private static string Key(int userId) => $"UserContext:{userId}";

        public UserContextService(IUserMeRepository repo, IMapper mapper, IMemoryCache cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task<UserMeDto> BuildUserContextAsync(int userId)
        {
            var cacheKey = Key(userId);
            if (_cache.TryGetValue<UserMeDto>(cacheKey, out var cached))
                return cached;

            var user = await _repo.GetUserWithFullContextAsync(userId)
                       ?? throw new BusinessException("Usuario no encontrado");

            // Roles activos y no eliminados (sin duplicados por Id)
            var roles = user.RolUsers?
                .Select(ru => ru.Rol)
                .Where(r => r != null && r.Active && !r.IsDeleted)
                .DistinctBy(r => r!.Id)
                .Cast<Rol>()
                .ToList() ?? new();

            var roleNames = roles.Select(r => r.Name!).ToList();

            // Forms permitidos por los roles (considerando solo RFP activos)
            var allowedForms = roles
                .SelectMany(r => r.RolFormPermissions ?? Enumerable.Empty<RolFormPermission>())
                .Where(rfp => rfp.Active && !rfp.IsDeleted)
                .Select(rfp => rfp.Form)
                .Where(f => f != null && f.Active && !f.IsDeleted)
                .DistinctBy(f => f!.Id)
                .Cast<Form>()
                .ToList();

            // Módulos (con forms filtrados por pertenencia al módulo)
            var modules = allowedForms
                .SelectMany(f => f.FormModules ?? Enumerable.Empty<FormModule>())
                .Select(fm => fm.Module)
                .Where(m => m != null && m.Active && !m.IsDeleted)
                .DistinctBy(m => m!.Id)
                .Cast<Module>()
                .OrderBy(m => m.Name)
                .Select(module =>
                {
                    var moduleDto = module.Adapt<MenuModuleDto>();

                    var moduleForms = allowedForms
                        .Where(f => f.FormModules.Any(fm => fm.ModuleId == module.Id))
                        .OrderBy(f => f.Name)
                        .Select(f =>
                        {
                            var formDto = f.Adapt<FormDto>();

                            var formPerms = roles
                                .SelectMany(r => r.RolFormPermissions ?? Enumerable.Empty<RolFormPermission>())
                                .Where(rfp => rfp.Active && !rfp.IsDeleted && rfp.FormId == f.Id && rfp.Permission != null)
                                .Select(rfp => NormalizePermission(rfp.Permission!.Name!))
                                .Distinct(StringComparer.OrdinalIgnoreCase)
                                .ToList();

                            formDto.Permissions = formPerms;
                            return formDto;
                        })
                        .ToList();

                    moduleDto.Forms = moduleForms;
                    return moduleDto;
                })
                .ToList();

            // Asigna PersonId
            var dto = new UserMeDto
            {
                Id = user.Id,
                PersonId = user.PersonId,
                FullName = string.Join(" ",
                             new[] { user.Person?.FirstName, user.Person?.LastName }
                             .Where(s => !string.IsNullOrWhiteSpace(s))).Trim(),
                Email = user.Email,
                Roles = roleNames,
                Menu = modules
            };

            _cache.Set(cacheKey, dto, _cacheTtl);
            return dto;
        }


        public void InvalidateCache(int userId) => _cache.Remove(Key(userId));

        private static string NormalizePermission(string p)
            => p.Trim().ToUpperInvariant().Replace(" ", "_");
    }
}
