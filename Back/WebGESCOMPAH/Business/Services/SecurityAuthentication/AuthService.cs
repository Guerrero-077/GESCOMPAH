using Business.Interfaces.Implements.SecurityAuthentication;
using Data.Interfaz.IDataImplement.Persons;
using Data.Interfaz.IDataImplement.SecurityAuthentication;
using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Entity.DTOs.Implements.SecurityAuthentication.Auth.RestPasword;
using Entity.DTOs.Implements.SecurityAuthentication.Me;
using Entity.DTOs.Implements.SecurityAuthentication.User;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Messaging.Interfaces;

namespace Business.Services.SecurityAuthentication
{
    /// <summary>
    /// Servicio de autenticación. El contexto de usuario (/me) lo construye UserContextService.
    /// </summary>
    public class AuthService(
        IPasswordHasher<User> passwordHasher,
        IUserRepository userData,
        ILogger<AuthService> logger,
        IRolUserRepository rolUserData,
        IMapper mapper,
        ISendCode emailService,
        IPasswordResetCodeRepository passwordResetRepo,
        //IValidatorService validator,
        IUserContextService userContextService,
        IPersonRepository personRepository
    ) : IAuthService
    {
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
        private readonly IUserRepository _userRepository = userData;
        private readonly IRolUserRepository _rolUserData = rolUserData;
        private readonly ILogger<AuthService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly ISendCode _emailService = emailService;
        private readonly IPasswordResetCodeRepository _passwordResetRepo = passwordResetRepo;
        //private readonly IValidatorService _validator = validator;
        private readonly IUserContextService _userContext = userContextService;
        private readonly IPersonRepository _personRepository = personRepository;

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

            // Invalida contexto /me
            _userContext.InvalidateCache(user.Id);
        }


        public async Task ChangePasswordAsync(ChangePasswordDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId)
                       ?? throw new BusinessException("Usuario no encontrado.");

            // Validar contraseña actual
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, dto.CurrentPassword);
            if (result == PasswordVerificationResult.Failed)
                throw new BusinessException("La contraseña actual es incorrecta.");

            // Hashear nueva contraseña
            user.Password = _passwordHasher.HashPassword(user, dto.NewPassword);

            await _userRepository.UpdateAsync(user);
        }

        // ✅ Ahora delega completamente en UserContextService:
        public Task<UserMeDto> BuildUserContextAsync(int userId)
            => _userContext.BuildUserContextAsync(userId);


        //public async Task<UserDto> RegisterAsync(RegisterDto dto)
        //{
        //    try
        //    {
        //        //await _validator.ValidateAsync(dto);

        //        if (await _userRepository.ExistsByEmailAsync(dto.Email))
        //            throw new BusinessException("El correo ya está registrado.");

        //        if (await _personRepository.ExistsByDocumentAsync(dto.Document))
        //            throw new BusinessException("Ya existe una persona con este número de documento.");

        //        var person = _mapper.Map<Person>(dto);
        //        var user = _mapper.Map<User>(dto);

        //        var hasher = new PasswordHasher<User>();
        //        user.Password = hasher.HashPassword(user, dto.Password);
        //        user.Person = person;

        //        await _userRepository.AddAsync(user);
        //        await _rolUserData.AsignateRolDefault(user);

        //        var createdUser = await _userRepository.GetByIdAsync(user.Id)
        //            ?? throw new BusinessException("Error interno: no se pudo recuperar el usuario tras registrarlo.");

        //        // ⚠️ Importante: invalidar cache del contexto
        //        _userContext.InvalidateCache(user.Id);

        //        return _mapper.Map<UserDto>(createdUser);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessException($"Error en el registro del usuario: {ex.Message}", ex);
        //    }
        //}



    }
}
