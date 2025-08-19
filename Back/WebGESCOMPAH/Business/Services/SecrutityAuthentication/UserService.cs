using Business.Interfaces.Implements.SecrutityAuthentication;
using Business.Repository;
using Data.Interfaz.IDataImplemenent.SecurityAuthentication;
using Data.Services.SecurityAuthentication;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.User;
using MapsterMapper;
using Microsoft.AspNetCore.Identity; // <-- importa esto
using Utilities.Exceptions;
using Utilities.Helpers.Business;

namespace Business.Services.SecrutityAuthentication
{
    public class UserService : BusinessGeneric<UserSelectDto, UserCreateDto, UserUpdateDto, User>, IUserService
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserRepository _users; // opcional pero útil si necesitas includes
        private readonly IRolUserRepository _rolUserRepository;

        public UserService(IUserRepository data, IMapper mapper, IPasswordHasher<User> passwordHasher, IRolUserRepository rolUserRepository)
            : base(data, mapper)
        {
            _passwordHasher = passwordHasher;
            _users = data;
            _rolUserRepository = rolUserRepository;
        }


        public override async Task<IEnumerable<UserSelectDto>> GetAllAsync()
        {
            try
            {
                var users = await _users.GetAllAsync();

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



        // CREATE: siempre hashear antes de guardar
        public async Task<UserCreateDto> CreateUserAsync(UserCreateDto dto)
        {
            BusinessValidationHelper.ThrowIfNull(dto, "El DTO no puede ser nulo.");

            var entity = _mapper.Map<User>(dto);
            entity.InitializeLogicalState();

            entity.Password = _passwordHasher.HashPassword(entity, dto.Password); // <-- hash

            var created = await Data.AddAsync(entity);
            return _mapper.Map<UserCreateDto>(created);
        }

        // UPDATE: hashear sólo si viene una nueva contraseña
        public override async Task<UserSelectDto> UpdateAsync(UserUpdateDto dto)
        {
            BusinessValidationHelper.ThrowIfNull(dto, "El DTO no puede ser nulo.");
            BusinessValidationHelper.ThrowIfZeroOrLess(dto.Id, "El ID debe ser mayor que cero.");

            // Carga actual (tracking la maneja Data.UpdateAsync via Find + SetValues)
            var existing = await Data.GetByIdAsync(dto.Id)
                           ?? throw new BusinessException($"Usuario {dto.Id} no encontrado.");

            existing.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                // Hash NUEVA contraseña
                existing.Password = _passwordHasher.HashPassword(existing, dto.Password);
            }

            var updated = await Data.UpdateAsync(existing);
            return _mapper.Map<UserSelectDto>(updated);
        }
    }
}
