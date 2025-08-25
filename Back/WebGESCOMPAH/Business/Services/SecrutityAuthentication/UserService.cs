using Business.Interfaces.Implements.SecrutityAuthentication;
using Business.Repository;
using Data.Interfaz.IDataImplemenent.Persons;
using Data.Interfaz.IDataImplemenent.SecurityAuthentication;
using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.User;
using Entity.Infrastructure.Context;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Utilities.Exceptions;

namespace Business.Services.SecrutityAuthentication
{
    public class UserService : BusinessGeneric<UserSelectDto, UserCreateDto, UserUpdateDto, User>, IUserService
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserRepository _userRepository; 
        private readonly IRolUserRepository _rolUserRepository;
        private readonly IPersonRepository _personRepository;
        private readonly ApplicationDbContext _context;

        public UserService(IUserRepository data, IMapper mapper, IPasswordHasher<User> passwordHasher, IRolUserRepository rolUserRepository, IPersonRepository personRepository, ApplicationDbContext contex)
            : base(data, mapper)
        {
            _passwordHasher = passwordHasher;
            _userRepository = data;
            _rolUserRepository = rolUserRepository; 
            _personRepository = personRepository;
            _context = contex;
        }


        public override async Task<IEnumerable<UserSelectDto>> GetAllAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();

                var result = new List<UserSelectDto>();

                foreach (var user in users)
                {
                    var dto = _mapper.Map<UserSelectDto>(user);
                    dto.Roles = await _rolUserRepository.GetRoleNamesByUserIdAsync(user.Id);    
                    result.Add(dto);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al obtener los registros.", ex);
            }
        }

        public async Task<UserSelectDto> CreateWithPersonAndRolesAsync(UserCreateDto dto)
        {

            try{

                // 1) Validación de unicidad de email
                if (string.IsNullOrWhiteSpace(dto.Email))
                    throw new BusinessException("El correo es requerido.");

                if (dto is UserCreateDto && string.IsNullOrWhiteSpace(dto.Password))
                    throw new BusinessException("La contraseña es requerida.");


                if (await _userRepository.ExistsByEmailAsync(dto.Email))
                    throw new BusinessException("El correo ya está registrado.");

                if (await _personRepository.ExistsByDocumentAsync(dto.Document))
                    throw new BusinessException("Ya existe una persona con este número de documento.");

                await using var tx = await _context.Database.BeginTransactionAsync();

                // 2) Mapear User + Person (NO setear PersonId manualmente)
                var person = _mapper.Map<Person>(dto);
                var user = _mapper.Map<User>(dto);

                // 3) Hash de password
                var hasher = new PasswordHasher<User>();
                user.Password = _passwordHasher.HashPassword(user, dto.Password);
                user.Person = person;


                // 4) Persistir (EF inserta Person y luego User)
                await _userRepository.AddAsync(user);

                // 5) Roles: por defecto o explícitos
                var roleIds = (dto.RoleIds ?? Array.Empty<int>()).Where(x => x > 0).Distinct().ToList();
                await _rolUserRepository.ReplaceUserRolesAsync(user.Id, roleIds);

                await tx.CommitAsync();

                // 6) Volver a leer el usuario con includes para mapear seguro
                var created = await _userRepository.GetByIdAsync(user.Id)
                              ?? throw new BusinessException("Error interno: no se pudo recuperar el usuario tras registrarlo.");

                // 7) Construir DTO de salida
                var result = _mapper.Map<UserSelectDto>(created);
                result.Roles = (await _rolUserRepository.GetRoleNamesByUserIdAsync(created.Id)).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error en el registro del usuario: {ex.Message}", ex);
            }
         
        }


        public async Task<UserSelectDto> UpdateWithPersonAndRolesAsync(UserUpdateDto dto)
        {
            try
            {
                // 1) Validaciones mínimas
                if (string.IsNullOrWhiteSpace(dto.Email))
                    throw new BusinessException("El correo es requerido.");

                // 2) Cargar usuario actual
                var user = await _userRepository.GetByIdAsync(dto.Id)
                           ?? throw new BusinessException("Usuario no encontrado.");

                // 3) Unicidad de email (excluyendo al propio usuario)
                var emailExists = await _userRepository.ExistsByEmailAsync(dto.Email);
                if (emailExists && !string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
                    throw new BusinessException("El correo ya está registrado.");


                if (await _personRepository.ExistsByDocumentAsync(dto.Document))
                    throw new BusinessException("Ya existe una persona con este número de documento.");


                await using var tx = await _context.Database.BeginTransactionAsync();

                // 4) Mapear cambios (NO tocar PersonId)
                _mapper.Map(dto, user);                // Email, etc. (Password se maneja aparte por config)
                if (user.Person is null) user.Person = new Person();   // defensa por si viniera nulo
                _mapper.Map(dto, user.Person);         // Datos de persona


                // 6) Guardar cambios de User/Person
                await _userRepository.UpdateAsync(user);

                // 7) Reemplazar roles (idempotente)
                var roleIds = (dto.RoleIds ?? Array.Empty<int>()).Where(x => x > 0).Distinct().ToList();
                await _rolUserRepository.ReplaceUserRolesAsync(user.Id, roleIds);

                await tx.CommitAsync();

                // 8) Volver a leer con includes para respuesta consistente
                var updated = await _userRepository.GetByIdAsync(user.Id)
                              ?? throw new BusinessException("Error interno: no se pudo recuperar el usuario actualizado.");

                // 9) Mapear salida
                var result = _mapper.Map<UserSelectDto>(updated);
                result.Roles = (await _rolUserRepository.GetRoleNamesByUserIdAsync(updated.Id)).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al actualizar el usuario: {ex.Message}", ex);
            }
        }



        //// CREATE: siempre hashear antes de guardar
        //public async Task<UserCreateDto> CreateUserAsync(UserCreateDto dto)
        //{
        //    BusinessValidationHelper.ThrowIfNull(dto, "El DTO no puede ser nulo.");

        //    var entity = _mapper.Map<User>(dto);
        //    entity.InitializeLogicalState();

        //    entity.Password = _passwordHasher.HashPassword(entity, dto.Password); // <-- hash

        //    var created = await Data.AddAsync(entity);
        //    return _mapper.Map<UserCreateDto>(created);
        //}

        //// UPDATE: hashear sólo si viene una nueva contraseña
        //public override async Task<UserSelectDto> UpdateAsync(UserUpdateDto dto)
        //{
        //    BusinessValidationHelper.ThrowIfNull(dto, "El DTO no puede ser nulo.");
        //    BusinessValidationHelper.ThrowIfZeroOrLess(dto.Id, "El ID debe ser mayor que cero.");

        //    // Carga actual (tracking la maneja Data.UpdateAsync via Find + SetValues)
        //    var existing = await Data.GetByIdAsync(dto.Id)
        //                   ?? throw new BusinessException($"Usuario {dto.Id} no encontrado.");

        //    existing.Email = dto.Email;

        //    if (!string.IsNullOrWhiteSpace(dto.Password))
        //    {
        //        // Hash NUEVA contraseña
        //        existing.Password = _passwordHasher.HashPassword(existing, dto.Password);
        //    }

        //    var updated = await Data.UpdateAsync(existing);
        //    return _mapper.Map<UserSelectDto>(updated);
        //}
    }
}
