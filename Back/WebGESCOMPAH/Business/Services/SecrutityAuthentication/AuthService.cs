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
using Microsoft.Extensions.Caching.Memory; // ⬅️ NUEVO
using System.Linq; // DistinctBy

namespace Business.Services.SecrutityAuthentication
{
    /// <summary>
    /// Servicio de autenticación y construcción de contexto de usuario (/me).
    /// Ahora con cache de /me mediante IMemoryCache (TTL por defecto: 10 minutos).
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
        IMemoryCache memoryCache // ⬅️ NUEVO
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
        private readonly IMemoryCache _cache = memoryCache; // ⬅️ NUEVO

        // Claves de cache (usa un prefijo para evitar colisiones)
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

                var createdUser = await _userRepository.GetByIdAsync(user.Id);
                if (createdUser is null)
                    throw new BusinessException("Error interno: no se pudo recuperar el usuario tras registrarlo.");

                // Invalida cache defensivamente por si hay lecturas inmediatas del usuario recién creado
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

            // Contraseña cambió → invalida /me por si tu UI muestra info derivada
            InvalidateUserCache(user.Id);
        }

        /// <summary>
        /// Construye el contexto de usuario (/me). Si existe en cache, lo devuelve.
        /// Politica: AbsoluteExpiration = 10 min (ajustable).
        /// </summary>
        public async Task<UserMeDto> BuildUserContextAsync(int userId)
        {
            var cacheKey = MeKey(userId);

            if (_cache.TryGetValue(cacheKey, out UserMeDto cached))
                return cached;

            // 1) Usuario con persona
            var user = await _IUserMeRepository.GetUserWithPersonAsync(userId)
                        ?? throw new BusinessException("Usuario no encontrado");

            // 2) Roles con permisos (estado actual)
            var userRoles = await _IUserMeRepository.GetUserRolesWithPermissionsAsync(userId);

            // 3) Filtrar roles activos y únicos
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

            // 4) Construir permisos por formulario y set global
            var formPermissions = new Dictionary<int, HashSet<string>>();
            var permissions = new HashSet<string>();

            foreach (var role in roles)
            {
                foreach (var rfp in role.RolFormPermissions)
                {
                    var pName = rfp.Permission?.Name;
                    if (string.IsNullOrWhiteSpace(pName)) continue;

                    permissions.Add(pName);

                    if (!formPermissions.ContainsKey(rfp.FormId))
                        formPermissions[rfp.FormId] = new();

                    formPermissions[rfp.FormId].Add(pName);
                }
            }

            // 5) Formularios + módulos
            var formIds = formPermissions.Keys.ToList();
            var formsWithModules = await _IUserMeRepository.GetFormsWithModulesByIdsAsync(formIds);

            // 6) Armar módulos con formularios y permisos
            var modules = formsWithModules
                .Where(f => f.FormModules.Any())
                .GroupBy(f => f.FormModules.First().Module)
                .Select(g =>
                {
                    var module = g.Key.Adapt<MenuModuleDto>();
                    module.Forms = g.Select(form =>
                    {
                        var dto = form.Adapt<FormDto>();
                        dto.Permissions = formPermissions[form.Id];
                        return dto;
                    }).ToList();

                    return module;
                }).ToList();

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

            // 8) Guardar en cache
            _cache.Set(cacheKey, me, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            return me;
        }

        /// <summary>
        /// Invalida el cache del contexto de usuario (/me). 
        /// Llama este método cuando actualices usuario, roles, permisos o menú.
        /// </summary>
        public void InvalidateUserCache(int userId)
        {
            _cache.Remove(MeKey(userId));
        }
    }
}
