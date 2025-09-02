using Business.CustomJWT;
using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.IDataImplement.Business;
using Data.Interfaz.IDataImplement.Persons;
using Data.Interfaz.IDataImplement.SecurityAuthentication;
using Data.Services.Business;
using Data.Services.Persons;
using Data.Services.SecurityAuthentication;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.Business.Contract;
using Entity.Infrastructure.Context;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Mail;
using Utilities.Exceptions;
using Utilities.Helpers.GeneratePassword;
using Utilities.Messaging.Implements;
using Utilities.Messaging.Interfaces;

namespace Business.Services.Business
{
    /// <summary>
    /// Crea contratos (persona/usuario opcional), asocia locales y marca sus establecimientos como ocupados (Active = false).
    /// Patrón aplicado: ExecutionStrategy + transacción local para soportar reintentos de EF Core con SQL Server.
    /// </summary>
    public class ContractService
        : BusinessGeneric<ContractSelectDto, ContractCreateDto, ContractUpdateDto, Contract>, IContractService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRolUserRepository _rolUserRepository;
        private readonly IPremisesLeasedRepository _premisesLeasedRepository;
        private readonly IEstablishmentsRepository _establishmentsRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ISendCode _emailService;
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUser _user;

        public ContractService(
            IContractRepository data,
            IPersonRepository personRepository,
            IUserRepository userRepository,
            IRolUserRepository rolUserRepository,
            IPremisesLeasedRepository premisesLeasedRepository,
            IEstablishmentsRepository establishmentsRepository,
            IMapper mapper,
            IPasswordHasher<User> passwordHasher,
            ISendCode emailService,
            ApplicationDbContext context,
            ICurrentUser user
        ) : base(data, mapper)
        {
            _personRepository = personRepository;
            _userRepository = userRepository;
            _rolUserRepository = rolUserRepository;
            _contractRepository = data;
            _premisesLeasedRepository = premisesLeasedRepository;
            _establishmentsRepository = establishmentsRepository;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
            _context = context;
            _user = user;
        }

        /// <summary>
        /// Lista para grilla (admin → todos; arrendador → solo los suyos).
        /// Proyección liviana: trae datos de persona y totales pactados.
        /// </summary>
        public async Task<IReadOnlyList<ContractCardDto>> GetMineAsync()
        {
            var list = _user.EsAdministrador
                ? await _contractRepository.GetCardsAllAsync()
                : await _contractRepository.GetCardsByPersonAsync(
                    _user.PersonId ?? throw new BusinessException("Usuario sin persona asociada.")
                  );

            var result = list.Select(c => new ContractCardDto(
                c.Id,
                c.PersonId,
                c.PersonFullName,
                c.PersonDocument,
                c.PersonPhone,
                c.PersonEmail,
                c.StartDate,
                c.EndDate,
                c.TotalBase,
                c.TotalUvt,
                c.Active
            )).ToList();

            return result.AsReadOnly();
        }

        // Proyección liviana -> suma totales sin materializar entidades completas.
        private async Task<(decimal totalBase, decimal totalUvt)> SumEstablishmentsAsync(IEnumerable<int> establishmentIds)
        {
            var ids = establishmentIds?.Distinct().ToList() ?? [];
            if (ids.Count == 0) return (0m, 0m);

            var basics = await _establishmentsRepository.GetBasicsByIdsAsync(ids); // <- Debe materializar en repo
            var totalBase = basics.Sum(b => b.RentValueBase);
            var totalUvt = basics.Sum(b => b.UvtQty);
            return (totalBase, totalUvt);
        }

        /// <summary>
        /// Crea contrato + (opcional) usuario, asocia locales y los marca inactivos.
        /// Usa ExecutionStrategy para que EF pueda reintentar la unidad transaccional completa sin lanzar
        /// "SqlServerRetryingExecutionStrategy no soporta transacciones de usuario".
        /// </summary>
        
    //  helper de validación (dentro de tu ContractService)
    private static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try { _ = new MailAddress(email); return true; }
        catch { return false; }
    }

    public async Task<int> CreateContractWithPersonHandlingAsync(ContractCreateDto dto)
    {
            if (dto.EstablishmentIds is null || dto.EstablishmentIds.Count == 0)
                throw new BusinessException("Debe seleccionar al menos un establecimiento.");

            var targetIds = dto.EstablishmentIds.Distinct().ToList();

            // Validación previa (fail-fast) si se pretende crear usuario
            var wantsUser = !string.IsNullOrWhiteSpace(dto.Email);
            if (wantsUser && !IsValidEmail(dto.Email))
                throw new BusinessException("El correo proporcionado no es válido.");

            var strategy = _context.Database.CreateExecutionStrategy();

            // Datos para notificación post-commit
            string? emailToNotify = null;
            string? fullNameToNotify = null;
            string? tempPassToNotify = null;

            var contractId = await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 0) Validación de disponibilidad
                    var alreadyInactive = await _establishmentsRepository.GetInactiveIdsAsync(targetIds);
                    if (alreadyInactive.Count > 0)
                        throw new BusinessException($"Los establecimientos {string.Join(", ", alreadyInactive)} no están disponibles (Active = false).");

                    // 1) Persona
                    var person = await _personRepository.GetByDocumentAsync(dto.Document);
                    if (person is null)
                    {
                        person = _mapper.Map<Person>(dto);
                        await _personRepository.AddAsync(person);
                        await _context.SaveChangesAsync();
                    }

                    // 2) Usuario opcional (crear sin enviar correo aquí)
                    var user = await _userRepository.GetByPersonIdAsync(person.Id);
                    if (user is null && wantsUser)
                    {
                        if (await _userRepository.ExistsByEmailAsync(dto.Email!))
                            throw new BusinessException("El correo ya está registrado.");

                        var tempPass = PasswordGenerator.Generate(12);
                        user = new User
                        {
                            Email = dto.Email!.Trim(),
                            PersonId = person.Id,
                            Password = _passwordHasher.HashPassword(null!, tempPass),
                            // Recomendado: RequirePasswordChange = true
                        };
                        await _userRepository.AddAsync(user);
                        await _rolUserRepository.AsignateRolDefault(user);
                        await _context.SaveChangesAsync();

                        // payload para el envío post-commit
                        tempPassToNotify = tempPass;
                        emailToNotify = user.Email;
                        fullNameToNotify = $"{person.FirstName} {person.LastName}".Trim();
                    }

                    // 3) Contrato
                    var contract = _mapper.Map<Contract>(dto);
                    contract.PersonId = person.Id;

                    // 4) Totales
                    var (totalBase, totalUvt) = await SumEstablishmentsAsync(targetIds);
                    contract.TotalBaseRentAgreed = totalBase;
                    contract.TotalUvtQtyAgreed = totalUvt;

                    await _contractRepository.AddAsync(contract);
                    await _context.SaveChangesAsync(); // genera Id

                    // 5) Premises
                    var premises = targetIds.Select(id => new PremisesLeased
                    {
                        ContractId = contract.Id,
                        EstablishmentId = id
                    }).ToList();
                    await _premisesLeasedRepository.AddRangeAsync(premises);
                    await _context.SaveChangesAsync();

                    // 6) Ocupa locales
                    var rows = await _establishmentsRepository.SetActiveByIdsAsync(targetIds, active: false);
                    if (rows != targetIds.Count)
                        throw new BusinessException("Conflicto de concurrencia al actualizar estados de establecimientos. Verifique disponibilidad.");

                    await tx.CommitAsync();
                    return contract.Id;
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }
            });

            // === POST-COMMIT: envío de email tolerante a fallos (no rompe el flujo) ===
            if (!string.IsNullOrWhiteSpace(emailToNotify) &&
                !string.IsNullOrWhiteSpace(fullNameToNotify) &&
                !string.IsNullOrWhiteSpace(tempPassToNotify))
            {
                try
                {
                    await _emailService.SendTemporaryPasswordAsync(emailToNotify!, fullNameToNotify!, tempPassToNotify!);
                }
                catch (Exception ex)
                {
                    // loguea y sigue; evita rollback del negocio por SMTP
                    // _logger.LogError(ex, "Fallo al enviar contraseña temporal a {Email}", emailToNotify);
                }
            }

            return contractId;
        }

    }
}
