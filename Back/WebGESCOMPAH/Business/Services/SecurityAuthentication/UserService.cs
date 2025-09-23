using Business.Interfaces.Implements.SecurityAuthentication;
using Business.Repository;
using Data.Interfaz.IDataImplement.Persons;
using Data.Interfaz.IDataImplement.SecurityAuthentication;
using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.User;
using Entity.Infrastructure.Context;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // CreateExecutionStrategy
using Utilities.Exceptions;
using Utilities.Helpers.GeneratePassword;
using Utilities.Messaging.Interfaces;

namespace Business.Services.SecurityAuthentication
{
    public class UserService
        : BusinessGeneric<UserSelectDto, UserCreateDto, UserUpdateDto, User>, IUserService
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserRepository _userRepository;
        private readonly IRolUserRepository _rolUserRepository;
        private readonly IPersonRepository _personRepository;
        private readonly ApplicationDbContext _context;
        private readonly ISendCode _emailService;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IPasswordHasher<User> passwordHasher,
            IRolUserRepository rolUserRepository,
            IPersonRepository personRepository,
            ISendCode emailService,
            ApplicationDbContext context
        ) : base(userRepository, mapper)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _rolUserRepository = rolUserRepository;
            _personRepository = personRepository;
            _context = context;
            _emailService = emailService;
        }

        // ======================================================
        // LECTURAS
        // ======================================================
        public override async Task<IEnumerable<UserSelectDto>> GetAllAsync()
        {
            // Deja que el repo optimice con AsNoTracking / Includes necesarios
            var users = await _userRepository.GetAllAsync();
            var result = new List<UserSelectDto>(capacity: users.Count());

            foreach (var u in users)
            {
                var dto = _mapper.Map<UserSelectDto>(u);
                dto.Roles = await _rolUserRepository.GetRoleNamesByUserIdAsync(u.Id);
                result.Add(dto);
            }
            return result;
        }

        // ======================================================
        // CREAR: Persona + Usuario + Roles (con contraseña temporal)
        // ======================================================
        public async Task<UserSelectDto> CreateWithPersonAndRolesAsync(UserCreateDto dto)
        {
            // --- Validaciones de dominio (-> 409) ---
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new BusinessException("El correo es requerido.");

            if (await _userRepository.ExistsByEmailAsync(dto.Email))
                throw new BusinessException("El correo ya está registrado.");

            if (await _personRepository.ExistsByDocumentAsync(dto.Document))
                throw new BusinessException("Ya existe una persona con este número de documento.");

            var strategy = _context.Database.CreateExecutionStrategy();
            string? tempPassword = null;

            await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _context.Database.BeginTransactionAsync();

                // 1) Mapear entidades
                var person = _mapper.Map<Person>(dto);

                var user = _mapper.Map<User>(dto);
                user.Person = person;

                // 2) Generar + hashear contraseña temporal
                tempPassword = PasswordGenerator.Generate(12);
                user.Password = _passwordHasher.HashPassword(user, tempPassword);

                // 3) Bandera de primer inicio (si existe en tu entidad)
                var mustChangeProp = typeof(User).GetProperty("MustChangePassword");
                if (mustChangeProp is not null)
                    mustChangeProp.SetValue(user, true);

                // 4) Guardar usuario
                await _userRepository.AddAsync(user);

                // 5) Reemplazar roles (idempotente)
                var roleIds = (dto.RoleIds ?? Array.Empty<int>()).Where(x => x > 0).Distinct().ToList();
                await _rolUserRepository.ReplaceUserRolesAsync(user.Id, roleIds);

                await tx.CommitAsync();
            });

            // 6) Side-effect fuera del bloque retriable (evita duplicados si hay reintentos)
            try
            {
                var fullName = $"{dto.FirstName} {dto.LastName}".Trim();
                if (!string.IsNullOrWhiteSpace(dto.Email) && !string.IsNullOrWhiteSpace(tempPassword))
                {
                    await _emailService.SendTemporaryPasswordAsync(dto.Email, fullName, tempPassword!);
                }
            }
            catch
            {
                // Loguea y no interrumpas la operación de negocio por fallo de email.
            }

            // 7) Respuesta consistente (lectura detallada, readonly)
            var userId = await _userRepository.GetIdByEmailAsync(dto.Email)
                         ?? throw new Exception("No se pudo recuperar el ID del usuario tras el registro.");

            var created = await _userRepository.GetByIdWithDetailsAsync(userId)
                          ?? throw new Exception("No se pudo recuperar el usuario tras registrarlo.");

            var result = _mapper.Map<UserSelectDto>(created);
            result.Roles = (await _rolUserRepository.GetRoleNamesByUserIdAsync(created.Id)).ToList();
            return result;
        }

        // ======================================================
        // ACTUALIZAR: Usuario + Persona + Roles (sin tocar Document)
        // ======================================================
        public async Task<UserSelectDto> UpdateWithPersonAndRolesAsync(UserUpdateDto dto)
        {
            // --- Reglas de negocio (→ 409) ---
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new BusinessException("El correo es requerido.");

            // Cargar User + Person (tracked) para actualizar sin crear una nueva Person
            var user = await _userRepository.GetByIdForUpdateAsync(dto.Id)
                       ?? throw new BusinessException("Usuario no encontrado.");

            // Unicidad de email excluyendo el propio Id
            if (await _userRepository.ExistsByEmailExcludingIdAsync(dto.Id, dto.Email))
                throw new BusinessException("El correo ya está registrado por otro usuario.");

            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _context.Database.BeginTransactionAsync();

                // 1) Mapear cambios en User
                _mapper.Map(dto, user);

                // 2) Mapear cambios en Person (FirstName, LastName, Phone, Address, CityId, etc.)
                if (user.Person is null)
                    throw new BusinessException("El usuario no tiene una persona asociada.");

                _mapper.Map(dto, user.Person);

                // Airbag: jamás persistir cambios en Person.Document desde este flujo
                _context.Entry(user.Person).Property(p => p.Document).IsModified = false;

                // 3) Persistir cambios
                await _userRepository.UpdateAsync(user);

                // 4) Reemplazar roles (idempotente)
                var roleIds = (dto.RoleIds ?? Array.Empty<int>()).Where(x => x > 0).Distinct().ToList();
                await _rolUserRepository.ReplaceUserRolesAsync(user.Id, roleIds);

                await tx.CommitAsync();
            });

            // 5) Respuesta consistente (lectura rica, readonly)
            var updated = await _userRepository.GetByIdWithDetailsAsync(user.Id)
                          ?? throw new Exception("No se pudo recuperar el usuario actualizado.");

            var result = _mapper.Map<UserSelectDto>(updated);
            result.Roles = (await _rolUserRepository.GetRoleNamesByUserIdAsync(updated.Id)).ToList();
            return result;
        }

        public async Task<(int userId, bool created, string? tempPassword)> EnsureUserForPersonAsync(int personId, string email)
        {
            if (personId <= 0)
                throw new BusinessException("PersonId invalido.");

            if (string.IsNullOrWhiteSpace(email))
                throw new BusinessException("El correo es requerido.");

            var normalizedEmail = email.Trim();

            var existing = await _userRepository.GetByPersonIdAsync(personId);
            if (existing is not null)
                return (existing.Id, false, null);

            if (await _userRepository.ExistsByEmailAsync(normalizedEmail))
                throw new BusinessException("El correo ya esta registrado.");

            if (await _personRepository.GetByIdAsync(personId) is null)
                throw new BusinessException("Persona no encontrada para crear el usuario.");

            var tempPassword = PasswordGenerator.Generate(12);

            var user = new User
            {
                Email = normalizedEmail,
                PersonId = personId
            };

            user.Password = _passwordHasher.HashPassword(user, tempPassword);

            await _userRepository.AddAsync(user);
            await _rolUserRepository.AsignateRolDefault(user);

            return (user.Id, true, tempPassword);
        }
    }
}
