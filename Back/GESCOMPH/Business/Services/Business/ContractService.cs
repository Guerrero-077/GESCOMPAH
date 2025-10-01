using Business.CustomJWT;
using Business.Interfaces.Implements.Business;
using Business.Interfaces.Implements.Persons;
using Business.Interfaces.Implements.SecurityAuthentication;
using Business.Repository;
using Business.Interfaces;
using Business.Interfaces.PDF;
using Data.Interfaz.IDataImplement.Business;
using Data.Interfaz.IDataImplement.Persons;
using Data.Interfaz.IDataImplement.SecurityAuthentication;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Persons;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.Business.Contract;
using Entity.DTOs.Implements.Business.ObligationMonth;
using Entity.DTOs.Implements.Persons.Person;
using Entity.Infrastructure.Context;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using Utilities.Exceptions;
using Utilities.Helpers.GeneratePassword;
using Utilities.Messaging.Interfaces;

namespace Business.Services.Business
{
    /// <summary>
    /// Servicio de contratos: orquesta el caso de uso y delega reglas a servicios existentes.
    /// TODO (seguridad): migrar password temporal a token de set-password cuando uses UserManager.
    /// </summary>
    public class ContractService
        : BusinessGeneric<ContractSelectDto, ContractCreateDto, ContractUpdateDto, Contract>, IContractService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IPersonService _personService;
        private readonly IPersonRepository _personRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRolUserRepository _rolUserRepository;
        private readonly IPremisesLeasedRepository _premisesLeasedRepository;
        private readonly IEstablishmentsRepository _establishmentsRepository;
        private readonly IEstablishmentService _establishmentService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ISendCode _emailService;
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUser _user;
        private readonly IObligationMonthRepository _obligationRepository;
        private readonly IObligationMonthService _obligationMonthSvc;
        private readonly IUserContextService _userContextService;
        private readonly IContractPdfGeneratorService _contractPdfService;
        private readonly ILogger<ContractService> _logger;

        public ContractService(
            IContractRepository contracts,
            IPersonService personService,
            IPersonRepository personRepository,
            IUserRepository userRepository,
            IRolUserRepository rolUserRepository,
            IPremisesLeasedRepository premisesLeasedRepository,
            IEstablishmentsRepository establishmentsRepository,
            IEstablishmentService establishmentService,
            IMapper mapper,
            IPasswordHasher<User> passwordHasher,
            ISendCode emailService,
            ApplicationDbContext context,
            ICurrentUser currentUser,
            IObligationMonthRepository obligationRepository,
            IObligationMonthService obligationMonthSvc,
            IUserContextService userContextService,
            IContractPdfGeneratorService contractPdfService,
            IUnitOfWork uow,
            ILogger<ContractService> logger
        ) : base(contracts, mapper)
        {
            _contractRepository = contracts;
            _personService = personService;
            _personRepository = personRepository;
            _userRepository = userRepository;
            _rolUserRepository = rolUserRepository;
            _premisesLeasedRepository = premisesLeasedRepository;
            _establishmentsRepository = establishmentsRepository;
            _establishmentService = establishmentService;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
            _context = context;
            _user = currentUser;
            _obligationRepository = obligationRepository;
            _obligationMonthSvc = obligationMonthSvc;
            _userContextService = userContextService;
            _contractPdfService = contractPdfService;
            _uow = uow;
            _logger = logger;
        }

        public async Task<IReadOnlyList<ContractCardDto>> GetMineAsync()
        {
            var list = _user.EsAdministrador
                ? await _contractRepository.GetCardsAllAsync()
                : await _contractRepository.GetCardsByPersonAsync(
                    _user.PersonId ?? throw new BusinessException("Usuario sin persona asociada.")
                  );

            return list.ToList().AsReadOnly();
        }


        private static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try { _ = new MailAddress(email); return true; }
            catch { return false; }
        }

        public async Task<int> CreateContractWithPersonHandlingAsync(ContractCreateDto dto)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            if (dto.EstablishmentIds is null || dto.EstablishmentIds.Count == 0)
                throw new BusinessException("Debe seleccionar al menos un establecimiento.");

            var targetIds = dto.EstablishmentIds.Distinct().ToList();
            var wantsUser = !string.IsNullOrWhiteSpace(dto.Email);
            if (wantsUser && !IsValidEmail(dto.Email))
                throw new BusinessException("El correo proporcionado no es válido.");

            // variables para notificaciones
            string? emailToNotify = null;
            string? fullNameToNotify = null;
            string? tempPassToNotify = null;
            bool userWasCreated = false;
            int personIdLocal = 0;

            // 👇 snapshot a usar en post-commit (sin tocar EF luego)
            ContractSelectDto? contractDtoSnapshot = null;

            var contractId = await _uow.ExecuteAsync<int>(async ct =>
            {
                // 1) disponibilidad
                var alreadyInactive = await _establishmentsRepository.GetInactiveIdsAsync(targetIds);
                if (alreadyInactive.Count > 0)
                    throw new BusinessException($"Los establecimientos {string.Join(", ", alreadyInactive)} no están disponibles (Active = false).");

                // 2) persona
                Person personEntity;
                var existingPerson = await _personService.GetByDocumentAsync(dto.Document);
                if (existingPerson is null)
                {
                    var personDto = _mapper.Map<PersonDto>(dto);
                    var created = await _personService.CreateAsync(personDto);
                    personEntity = await _context.Set<Person>().FindAsync(created.Id)
                                   ?? throw new BusinessException("No se pudo materializar la persona creada.");
                }
                else
                {
                    personEntity = await _context.Set<Person>().FindAsync(existingPerson.Id)
                                   ?? throw new BusinessException("Persona no encontrada en el contexto.");
                }

                // 3) usuario opcional
                User? createdUser = null;
                if (wantsUser)
                {
                    var existingUser = await _userRepository.GetByPersonIdAsync(personEntity.Id);
                    if (existingUser is null)
                    {
                        if (await _userRepository.ExistsByEmailAsync(dto.Email!))
                            throw new BusinessException("El correo ya está registrado.");

                        var tempPass = PasswordGenerator.Generate(12);
                        createdUser = new User
                        {
                            Email = dto.Email!.Trim(),
                            PersonId = personEntity.Id,
                            Password = _passwordHasher.HashPassword(null!, tempPass),
                        };

                        await _userRepository.AddAsync(createdUser);
                        await _rolUserRepository.AsignateRolDefault(createdUser);

                        tempPassToNotify = tempPass;
                        emailToNotify = createdUser.Email;
                        fullNameToNotify = $"{personEntity.FirstName} {personEntity.LastName}".Trim();
                        userWasCreated = true;

                        _uow.RegisterPostCommit(async _ =>
                        {
                            try
                            {
                                if (!string.IsNullOrWhiteSpace(emailToNotify) &&
                                    !string.IsNullOrWhiteSpace(fullNameToNotify) &&
                                    !string.IsNullOrWhiteSpace(tempPassToNotify))
                                {
                                    await _emailService.SendTemporaryPasswordAsync(emailToNotify!, fullNameToNotify!, tempPassToNotify!);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error enviando contraseña temporal a {Email} para usuario {FullName}", emailToNotify, fullNameToNotify);
                            }
                        });
                    }
                }

                // 4) totales
                var basics = await _establishmentService.GetBasicsByIdsAsync(targetIds);
                var totalBase = basics.Sum(b => b.RentValueBase);
                var totalUvt = basics.Sum(b => b.UvtQty);

                // 5) contrato + locales
                var contract = _mapper.Map<Contract>(dto);
                contract.PersonId = personEntity.Id;
                contract.TotalBaseRentAgreed = totalBase;
                contract.TotalUvtQtyAgreed = totalUvt;

                foreach (var estId in targetIds)
                    contract.PremisesLeased.Add(new PremisesLeased { EstablishmentId = estId });

                await _contractRepository.AddAsync(contract);

                // 5.1) cláusulas
                if (dto.ClauseIds is { Count: > 0 })
                {
                    var uniqueClauseIds = dto.ClauseIds.Distinct().ToList();
                    var links = uniqueClauseIds.Select(cid => new ContractClause { ContractId = contract.Id, ClauseId = cid });
                    await _context.contractClauses.AddRangeAsync(links);
                }

                // 6) desactivar establecimientos
                var rows = await _establishmentsRepository.SetActiveByIdsAsync(targetIds, active: false);
                if (rows != targetIds.Count)
                    throw new BusinessException("Conflicto de concurrencia al actualizar estados de establecimientos.");

                // 7) persistir
                await _context.SaveChangesAsync();

                // 🔎 CARGA para SNAPSHOT DTO **antes** de cerrar el contexto
                // Si tienes un método “con includes”, úsalo:
                // var fullEntity = await _contractRepository.GetByIdWithDetailsAsync(contract.Id);
                // contractDtoSnapshot = _mapper.Map<ContractSelectDto>(fullEntity);

                // Alternativa segura si tu mapeo de Mapster alcanza con lo ya materializado:
                await _context.Entry(contract).Collection(c => c.PremisesLeased).LoadAsync();
                // Cargar lo que tu PDF necesite (ejemplos):
                foreach (var pl in contract.PremisesLeased)
                    await _context.Entry(pl).Reference(x => x.Establishment).LoadAsync();
                contractDtoSnapshot = _mapper.Map<ContractSelectDto>(contract);

                // Post-commit: invalidar caché si hubo user nuevo
                _uow.RegisterPostCommit(_ =>
                {
                    if (createdUser is not null && createdUser.Id > 0)
                    {
                        try { _userContextService.InvalidateCache(createdUser.Id); }
                        catch (Exception ex) { _logger.LogError(ex, "Error invalidando caché para usuario {UserId}", createdUser.Id); }
                    }
                    return Task.CompletedTask;
                });

                // Post-commit: obligación mensual
                _uow.RegisterPostCommit(async _ =>
                {
                    var tz = TimeZoneConverter.TZConvert.GetTimeZoneInfo("America/Bogota");
                    var nowLocal = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);
                    try { await _obligationMonthSvc.GenerateForContractMonthAsync(contract.Id, nowLocal.Year, nowLocal.Month); }
                    catch (Exception ex) { _logger.LogError(ex, "Error generando obligación mensual para contrato {ContractId}", contract.Id); }
                });

                // Post-commit: PDF + correo SIN tocar EF
                if (!string.IsNullOrWhiteSpace(dto.Email) && contractDtoSnapshot is not null)
                {
                    var toEmail = dto.Email!;
                    var fullName = $"{personEntity.FirstName} {personEntity.LastName}".Trim();
                    var contractNumber = contract.Id.ToString("D6");

                    _uow.RegisterPostCommit(async _ =>
                    {
                        try
                        {
                            var pdfBytes = await _contractPdfService.GeneratePdfAsync(contractDtoSnapshot!);
                            await _emailService.SendContractWithPdfAsync(toEmail, fullName, contractNumber, pdfBytes);
                            _logger.LogInformation("Correo con PDF del contrato enviado a {Email} para contrato {ContractId}", toEmail, contract.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error enviando correo con PDF del contrato {ContractId} a {Email}", contract.Id, toEmail);
                        }
                    });
                }

                personIdLocal = personEntity.Id;
                return contract.Id;
            });

            sw.Stop();
            _logger.LogInformation("Contract {ContractId} created in {ElapsedMs} ms for person {PersonId} with {Count} establishments. UserCreated={UserCreated}",
                contractId, sw.ElapsedMilliseconds, personIdLocal, targetIds.Count, userWasCreated);

            return contractId;
        }


        public async Task<ExpirationSweepResult> RunExpirationSweepAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var strategy = _context.Database.CreateExecutionStrategy();
            var sw = System.Diagnostics.Stopwatch.StartNew();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _context.Database.BeginTransactionAsync(ct);
                try
                {
                    var deactivatedIds = await _contractRepository.DeactivateExpiredAsync(now);
                    var reactivated = await _contractRepository.ReleaseEstablishmentsForExpiredAsync(now);

                    await tx.CommitAsync(ct);
                    sw.Stop();
                    _logger.LogInformation("Expiration sweep deactivated {Count} contracts and reactivated {Establishments} establishments in {ElapsedMs} ms",
                        deactivatedIds.Count, reactivated, sw.ElapsedMilliseconds);
                    return new ExpirationSweepResult(deactivatedIds, reactivated);
                }
                catch
                {
                    await tx.RollbackAsync(ct);
                    throw;
                }
            });
        }

        public async Task<IReadOnlyList<ObligationMonthSelectDto>> GetObligationsAsync(int contractId)
        {
            if (contractId <= 0) throw new BusinessException("contractId inválido.");

            var query = _obligationRepository.GetByContractQueryable(contractId);
            var list = await query.ToListAsync();
            return _mapper.Map<List<ObligationMonthSelectDto>>(list).AsReadOnly();
        }
    }
}
