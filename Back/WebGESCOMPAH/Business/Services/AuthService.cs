using Business.Interfaces.Implements;
using Common.Custom;
using Data.Interfaz.IDataImplemenent;
using Data.Interfaz.Security;
using Data.Services.SecurityAuthentication;
using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.AdministrationSystem;
using Entity.DTOs.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Entity.DTOs.Implements.SecurityAuthentication.Auth.RestPasword;
using Entity.DTOs.Interfaces;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Messaging.Interfaces;

namespace Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserMeRepository _IUserMeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRolUserRepository _rolUserData;
        private readonly ILogger<AuthService> _logger;
        private readonly IMapper _mapper;
        private readonly ISendCode _emailService;
        private readonly IPasswordResetCodeRepository _passwordResetRepo;
        private readonly IValidatorService _validator;

        public AuthService(
            IUserRepository userData,
            ILogger<AuthService> logger,
            IRolUserRepository rolUserData,
            IMapper mapper,
            ISendCode emailService,
            IPasswordResetCodeRepository passwordResetRepo,
            IValidatorService validator,
            IUserMeRepository IUserMeRepository
            )
        {
            _logger = logger;
            _userRepository = userData;
            _rolUserData = rolUserData;
            _mapper = mapper;
            _emailService = emailService;
            _passwordResetRepo = passwordResetRepo;
            _validator = validator;
            _IUserMeRepository = IUserMeRepository;
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            try
            {
                await _validator.ValidateAsync(dto);
                // Validar que el correo no esté registrado
                if (await _userRepository.ExistsByEmailAsync(dto.Email))
                    throw new Exception("Correo ya registrado");

                // Mapear DTO a entidades
                var person = _mapper.Map<Entity.Domain.Models.Implements.Persons.Person>(dto);
                var user = _mapper.Map<User>(dto);

                // Encriptar contraseña
                user.Password = EncriptePassword.EncripteSHA256(user.Password);

                // Asignar relación
                user.Person = person;


                // Guardar usuario
                await _userRepository.AddAsync(user);

                // Asignar rol por defecto
                await _rolUserData.AsignateRolDefault(user);

                // Recuperar el usuario con sus relaciones para el mapeo correcto
                var createduser = await _userRepository.GetByIdAsync(user.Id);
                if (createduser == null)
                    throw new BusinessException("Error interno: el usuario no pudo ser recuperado tras la creación.");

                return _mapper.Map<UserDto>(createduser);
            }
            catch (Exception ex)
            {
                // Aquí puedes loguear el error si tienes logger inyectado
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

            user.Password = EncriptePassword.EncripteSHA256(dto.NewPassword);
            await _userRepository.UpdateAsync(user);

            record.IsUsed = true;
            await _passwordResetRepo.UpdateAsync(record);
        }





        /// <summary>
        /// Construye el contexto de usuario con información detallada, roles, permisos y estructura de menú.
        /// </summary>
        /// <param name="userId">Identificador único del usuario.</param>
        /// <returns>
        /// Objeto <see cref="UserMeDto"/> con toda la información contextual del usuario.
        /// </returns>
        /// <exception cref="BusinessException">
        /// Se lanza cuando no se encuentra el usuario especificado.
        /// </exception>
        /// <remarks>
        /// Este método:
        /// 1. Obtiene datos básicos del usuario
        /// 2. Recupera y filtra roles activos
        /// 3. Construye la estructura de permisos
        /// 4. Organiza los módulos y formularios para el menú
        /// </remarks>
        public async Task<UserMeDto> BuildUserContextAsync(int userId)
        {
            // 1. Obtener usuario principal (con patrón null-coalescing throw)
            var user = await _IUserMeRepository.GetUserWithPersonAsync(userId)
                        ?? throw new BusinessException("Usuario no encontrado");

            // 2. Obtener roles con sus permisos
            var userRoles = await _IUserMeRepository.GetUserRolesWithPermissionsAsync(userId);

            // 3. Filtrar roles activos no eliminados (con DistinctBy para evitar duplicados)
            var roles = userRoles
                .Where(ur => ur.Active && !ur.IsDeleted)
                .Select(ur => ur.Rol)
                .Where(r => r.Active && !r.IsDeleted)
                .DistinctBy(r => r.Id)
                .ToList();

            var roleNames = roles.Select(r => r.Name).ToList();

            // 4. Construir diccionario de permisos:
            //    - Key: ID de formulario
            //    - Value: Conjunto de permisos para ese formulario
            var formPermissions = new Dictionary<int, HashSet<string>>();
            var permissions = new HashSet<string>();

            foreach (var role in roles)
            {
                foreach (var rfp in role.RolFormPermissions)
                {
                    if (string.IsNullOrWhiteSpace(rfp.Permission?.Name)) continue;

                    permissions.Add(rfp.Permission.Name);

                    if (!formPermissions.ContainsKey(rfp.FormId))
                        formPermissions[rfp.FormId] = new();

                    formPermissions[rfp.FormId].Add(rfp.Permission.Name);
                }
            }

            // 5. Obtener formularios con sus módulos
            var formIds = formPermissions.Keys.ToList();
            var formsWithModules = await _IUserMeRepository.GetFormsWithModulesByIdsAsync(formIds);

            // 6. Organizar módulos con sus formularios y permisos
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

            // 7. Construir DTO final
            return new UserMeDto
            {
                Id = user.Id,
                FullName = $"{user.Person.FirstName} {user.Person.LastName}",
                Email = user.Email,
                Roles = roleNames,
                Permissions = permissions.ToList(),
                Menu = modules
            };
        }
    }
}