using Business.Interfaces.Implements.SecrutityAuthentication;
using Data.Interfaz.IDataImplemenent.SecurityAuthentication;
using Data.Interfaz.Security;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Entity.DTOs.Implements.SecurityAuthentication.Auth.RestPasword;
using Entity.DTOs.Implements.SecurityAuthentication.Me;
using Entity.DTOs.Interfaces;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Messaging.Interfaces;
using Microsoft.Extensions.Caching.Memory; // cache /me
using System.Linq;

namespace Business.Services.SecrutityAuthentication
{
    /// <summary>
    /// Servicio de autenticación y construcción de contexto de usuario (/me) con IMemoryCache.
    /// </summary>
    public class AuthService(
        IUserRepository userData,
        ILogger<AuthService> logger,
        IRolUserRepository rolUserData,
        IMapper mapper,
        ISendCode emailService,
        IPasswordResetCodeRepository passwordResetRepo,
        IValidatorService validator,
        IUserMeRepository IUserMeRepository,
        IMemoryCache memoryCache
    ) : IAuthService
    {
        private readonly IUserMeRepository _IUserMeRepository = IUserMeRepository;
        private readonly IUserRepository _userRepository = userData;
        private readonly IRolUserRepository _rolUserData = rolUserData;
        private readonly ILogger<AuthService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly ISendCode _emailService = emailService;
        private readonly IPasswordResetCodeRepository _passwordResetRepo = passwordResetRepo;
        private readonly IValidatorService _validator = validator;
        private readonly IMemoryCache _cache = memoryCache;

        private static string MeKey(int userId) => $"me:{userId}";

        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            try
            {
                await _validator.ValidateAsync(dto);

                if (await _userRepository.ExistsByEmailAsync(dto.Email))
                    throw new BusinessException("El correo ya está registrado.");

                var person = _mapper.Map<Entity.Domain.Models.Implements.Persons.Person>(dto);
                var user = _mapper.Map<User>(dto);

                var hasher = new PasswordHasher<User>();
                user.Password = hasher.HashPassword(user, dto.Password);
                user.Person = person;

                await _userRepository.AddAsync(user);
                await _rolUserData.AsignateRolDefault(user);

                var createdUser = await _userRepository.GetByIdAsync(user.Id)
                    ?? throw new BusinessException("Error interno: no se pudo recuperar el usuario tras registrarlo.");

                InvalidateUserCache(user.Id);
                return _mapper.Map<UserDto>(createdUser);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error en el registro del usuario: {ex.Message}", ex);
            }
        }

        public async Task RequestPasswordResetAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email)
                ?? throw new ValidationException("Correo no registrado");

            var code = new Random().Next(100000, 999999).ToString();

            var resetCode = new PasswordResetCode
            {
                Email = email,
                Code = code,
                Expiration = DateTime.UtcNow.AddMinutes(10)
            };

            await _passwordResetRepo.AddAsync(resetCode);
            await _emailService.SendRecoveryCodeEmail(email, code);
        }

        public async Task ResetPasswordAsync(ConfirmResetDto dto)
        {
            var record = await _passwordResetRepo.GetValidCodeAsync(dto.Email, dto.Code)
                ?? throw new ValidationException("Código inválido o expirado");

            var user = await _userRepository.GetByEmailAsync(dto.Email)
                ?? throw new ValidationException("Usuario no encontrado");

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, dto.NewPassword);

            await _userRepository.UpdateAsync(user);

            record.IsUsed = true;
            await _passwordResetRepo.UpdateAsync(record);

            InvalidateUserCache(user.Id);
        }

        public async Task<UserMeDto> BuildUserContextAsync(int userId)
        {
            var cacheKey = MeKey(userId);
            if (_cache.TryGetValue(cacheKey, out UserMeDto cached))
                return cached;

            // 1) Usuario
            var user = await _IUserMeRepository.GetUserWithPersonAsync(userId)
                        ?? throw new BusinessException("Usuario no encontrado");

            // 2) Roles con permisos
            var userRoles = await _IUserMeRepository.GetUserRolesWithPermissionsAsync(userId);

            // 3) Roles activos únicos
            var roles = userRoles
                .Where(ur => ur.Active && !ur.IsDeleted)
                .Select(ur => ur.Rol)
                .Where(r => r.Active && !r.IsDeleted)
                .DistinctBy(r => r.Id)
                .ToList();

            var roleNames = roles.Select(r => r.Name)
                                 .Where(n => !string.IsNullOrWhiteSpace(n))
                                 .Distinct()
                                 .ToList();

            // 4) Permisos por formulario (solo RFP activos)
            var formPermissions = new Dictionary<int, HashSet<string>>();
            var permissions = new HashSet<string>();

            foreach (var role in roles)
            {
                foreach (var rfp in role.RolFormPermissions.Where(x => x.Active && !x.IsDeleted))
                {
                    var pName = rfp.Permission?.Name;
                    if (string.IsNullOrWhiteSpace(pName)) continue;

                    permissions.Add(pName);

                    if (!formPermissions.ContainsKey(rfp.FormId))
                        formPermissions[rfp.FormId] = new();

                    formPermissions[rfp.FormId].Add(pName);
                }
            }

            // 5) Formularios + módulos (solo activos)
            var formIds = formPermissions.Keys.ToList();
            var formsWithModules = await _IUserMeRepository.GetFormsWithModulesByIdsAsync(formIds);

            // 6) Armar módulos sin perder asociaciones múltiples
            var modules = formsWithModules
                .Where(f => f.Active && !f.IsDeleted)
                .SelectMany(f => f.FormModules
                    .Where(fm => fm.Active && !fm.IsDeleted && fm.Module != null && fm.Module.Active && !fm.Module.IsDeleted)
                    .Select(fm => new { Form = f, Module = fm.Module! }))
                .GroupBy(x => x.Module)
                .Select(g =>
                {
                    var module = g.Key.Adapt<MenuModuleDto>();
                    module.Forms = g
                        .Select(x =>
                        {
                            var dto = x.Form.Adapt<FormDto>();
                            dto.Permissions = formPermissions.TryGetValue(x.Form.Id, out var set) ? set : new HashSet<string>();
                            return dto;
                        })
                        .DistinctBy(d => d.Id)
                        .ToList();
                    return module;
                })
                .ToList();

            // 7) DTO final
            var me = new UserMeDto
            {
                Id = user.Id,
                FullName = $"{user.Person.FirstName} {user.Person.LastName}",
                Email = user.Email,
                Roles = roleNames,
                Permissions = permissions.ToList(),
                Menu = modules
            };

            // 8) Cache
            _cache.Set(cacheKey, me, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            return me;
        }

        public void InvalidateUserCache(int userId) => _cache.Remove(MeKey(userId));
    }
}
