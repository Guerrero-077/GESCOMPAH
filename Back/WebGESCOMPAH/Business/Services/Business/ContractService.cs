using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.CustomJWT;
using Business.Interfaces;
using Business.Interfaces.Implements.Business;
using Business.Interfaces.Implements.Persons;
using Business.Interfaces.Implements.SecurityAuthentication;
using Business.Interfaces.PDF;
using Business.Repository;
using Business.Services.Validation;
using Data.Interfaz.IDataImplement.Business;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Contract;
using Entity.DTOs.Implements.Business.ObligationMonth;
using Entity.DTOs.Implements.Persons.Person;
using Entity.Infrastructure.Context;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
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
        private readonly IEstablishmentService _establishmentService;
        private readonly IUserService _userService;
        private readonly ISendCode _emailService;
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUser _user;
        private readonly IObligationMonthService _obligationMonthSvc;
        private readonly IUserContextService _userContextService;
        private readonly IContractPdfGeneratorService _contractPdfService;
        private readonly ILogger<ContractService> _logger;
        private static readonly TimeZoneInfo BogotaTimeZone = TimeZoneConverter.TZConvert.GetTimeZoneInfo("America/Bogota");

        public ContractService(
            IContractRepository contracts,
            IPersonService personService,
            IEstablishmentService establishmentService,
            IUserService userService,
            IMapper mapper,
            ISendCode emailService,
            ApplicationDbContext context,
            ICurrentUser currentUser,
            IObligationMonthService obligationMonthSvc,
            IUserContextService userContextService,
            IContractPdfGeneratorService contractPdfService,
            IUnitOfWork uow,
            ILogger<ContractService> logger
        ) : base(contracts, mapper)
        {
            _contractRepository = contracts;
            _personService = personService;
            _establishmentService = establishmentService;
            _userService = userService;
            _emailService = emailService;
            _context = context;
            _user = currentUser;
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
        

        public async Task<int> CreateContractWithPersonHandlingAsync(ContractCreateDto dto)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            var sanitized = SanitizeContractCreateDto(dto);
            var establishmentIds = sanitized.EstablishmentIds;
            EnsureEstablishmentsSelected(establishmentIds);

            var personPayload = _mapper.Map<PersonDto>(sanitized);

            var personIdLocal = 0;
            var userWasCreated = false;

            var contractId = await _uow.ExecuteAsync<int>(async ct =>
            {
                var personSnapshot = await _personService.GetOrCreateByDocumentAsync(personPayload);
                personIdLocal = personSnapshot.Id;
                var personFullName = ComposeFullName(personSnapshot);

                var (totalBaseRent, totalUvt) = await _establishmentService.ReserveForContractAsync(establishmentIds);
                var userResult = await EnsureUserAsync(personSnapshot.Id, sanitized.Email);
                userWasCreated = userResult.Created;

                var contract = BuildContractEntity(sanitized, personSnapshot.Id, totalBaseRent, totalUvt, establishmentIds);
                await _contractRepository.AddAsync(contract);

                var contractSnapshot = await BuildContractSnapshotAsync(contract);
                ScheduleUserPostCommit(userResult, personFullName);
                ScheduleObligationPostCommit(contract.Id);
                ScheduleContractEmail(contract.Id, sanitized.Email, personFullName, contractSnapshot);

                return contract.Id;
            });

            sw.Stop();
            _logger.LogInformation(
                "Contract {ContractId} created in {ElapsedMs} ms for person {PersonId} with {Count} establishments. UserCreated={UserCreated}",
                contractId,
                sw.ElapsedMilliseconds,
                personIdLocal,
                establishmentIds.Count,
                userWasCreated);

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
                    _logger.LogInformation(
                        "Expiration sweep deactivated {Count} contracts and reactivated {Establishments} establishments in {ElapsedMs} ms",
                        deactivatedIds.Count,
                        reactivated,
                        sw.ElapsedMilliseconds);
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
            if (contractId <= 0) throw new BusinessException("contractId invalido.");

            return await _obligationMonthSvc.GetByContractAsync(contractId);
        }

        private static ContractCreateDto SanitizeContractCreateDto(ContractCreateDto dto)
        {
            if (dto is null)
                throw new BusinessException("Payload inválido.");

            var firstName = DomainValidation.RequireName(dto.FirstName, "Primer nombre", 50);
            var lastName = DomainValidation.RequireName(dto.LastName, "Apellido", 50);
            var document = DomainValidation.RequireColombianDocument(dto.Document);
            var phone = DomainValidation.RequireColombianPhone(dto.Phone);
            var address = DomainValidation.NormalizeAddress(dto.Address, required: true, maxLength: 150);

            DomainValidation.EnsureCityId(dto.CityId);

            var email = DomainValidation.NormalizeEmail(dto.Email);
            DomainValidation.EnsureValidEmail(email);

            var startDate = dto.StartDate.Date;
            var endDate = dto.EndDate.Date;
            DomainValidation.EnsureDateRange(startDate, endDate, "La fecha de inicio", "La fecha de finalización");
            DomainValidation.EnsureStartNotInPast(startDate, "La fecha de inicio", BogotaTimeZone);

            var establishmentIds = DomainValidation.NormalizePositiveIds(dto.EstablishmentIds);
            var clauseIds = DomainValidation.NormalizePositiveIds(dto.ClauseIds);

            return new ContractCreateDto
            {
                FirstName = firstName,
                LastName = lastName,
                Document = document,
                Phone = phone,
                Address = address,
                CityId = dto.CityId,
                Email = email,
                StartDate = startDate,
                EndDate = endDate,
                EstablishmentIds = establishmentIds,
                UseSystemParameters = dto.UseSystemParameters,
                ClauseIds = clauseIds
            };
        }
        private static void EnsureEstablishmentsSelected(IReadOnlyCollection<int> establishmentIds)
        {
            if (establishmentIds.Count == 0)
                throw new BusinessException("Debe seleccionar al menos un establecimiento.");
        }

        private static string ComposeFullName(PersonSelectDto person)
        {
            return string.Join(" ", new[] { person.FirstName, person.LastName }
                .Where(s => !string.IsNullOrWhiteSpace(s))).Trim();
        }

        private async Task<(bool Created, int UserId, string? TempPassword, string? Email)> EnsureUserAsync(int personId, string? emailCandidate)
        {
            if (string.IsNullOrWhiteSpace(emailCandidate))
                return (false, 0, null, null);

            var (userId, created, tempPassword) = await _userService.EnsureUserForPersonAsync(personId, emailCandidate);
            return (created, userId, tempPassword, emailCandidate);
        }

        private Contract BuildContractEntity(
            ContractCreateDto dto,
            int personId,
            decimal totalBaseRent,
            decimal totalUvt,
            IReadOnlyCollection<int> establishmentIds)
        {
            var contract = _mapper.Map<Contract>(dto);
            contract.PersonId = personId;
            contract.TotalBaseRentAgreed = totalBaseRent;
            contract.TotalUvtQtyAgreed = totalUvt;

            foreach (var estId in establishmentIds)
            {
                contract.PremisesLeased.Add(new PremisesLeased { EstablishmentId = estId });
            }

            if (dto.ClauseIds is { Count: > 0 })
            {
                foreach (var clauseId in dto.ClauseIds.Distinct())
                {
                    contract.ContractClauses.Add(new ContractClause { ClauseId = clauseId });
                }
            }

            return contract;
        }

        private async Task<ContractSelectDto?> BuildContractSnapshotAsync(Contract contract)
        {
            var loaded = await _contractRepository.GetByIdAsync(contract.Id);
            var source = loaded ?? contract;
            return _mapper.Map<ContractSelectDto>(source);
        }

        private void ScheduleUserPostCommit((bool Created, int UserId, string? TempPassword, string? Email) userResult, string personFullName)
        {
            if (!userResult.Created) return;

            _uow.RegisterPostCommit(ct =>
            {
                try
                {
                    _userContextService.InvalidateCache(userResult.UserId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error invalidando cache para usuario {UserId}", userResult.UserId);
                }

                return Task.CompletedTask;
            });

            if (string.IsNullOrWhiteSpace(userResult.TempPassword) || string.IsNullOrWhiteSpace(userResult.Email))
                return;

            RunPostCommitBackground(
                () => _emailService.SendTemporaryPasswordAsync(userResult.Email!, personFullName, userResult.TempPassword!),
                "Error enviando contrasena temporal a {Email} para usuario {FullName}",
                userResult.Email,
                personFullName);
        }

        private void ScheduleObligationPostCommit(int contractId)
        {
            _uow.RegisterPostCommit(async _ =>
            {
                var tz = TimeZoneConverter.TZConvert.GetTimeZoneInfo("America/Bogota");
                var nowLocal = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);
                try
                {
                    await _obligationMonthSvc.GenerateForContractMonthAsync(contractId, nowLocal.Year, nowLocal.Month);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generando obligacion mensual para contrato {ContractId}", contractId);
                }
            });
        }

        private void ScheduleContractEmail(int contractId, string? emailCandidate, string personFullName, ContractSelectDto? contractSnapshot)
        {
            if (string.IsNullOrWhiteSpace(emailCandidate) || contractSnapshot is null)
                return;

            var toEmail = emailCandidate!;
            var contractNumber = contractId.ToString("D6");

            RunPostCommitBackground(
                async () =>
                {
                    var pdfBytes = await _contractPdfService.GeneratePdfAsync(contractSnapshot);
                    await _emailService.SendContractWithPdfAsync(toEmail, personFullName, contractNumber, pdfBytes);
                    _logger.LogInformation("Correo con PDF del contrato enviado a {Email} para contrato {ContractId}", toEmail, contractId);
                },
                "Error enviando correo con PDF del contrato {ContractId} a {Email}",
                contractId,
                toEmail);
        }

        private void RunPostCommitBackground(Func<Task> action, string errorMessage, params object?[] contextArgs)
        {
            if (action is null) return;

            _uow.RegisterPostCommit(ct =>
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await action();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, errorMessage, contextArgs);
                    }
                });

                return Task.CompletedTask;
            });
        }
    }
}

