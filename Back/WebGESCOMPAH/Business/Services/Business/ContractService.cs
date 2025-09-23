using Business.CustomJWT;
using Business.Interfaces;
using Business.Interfaces.Implements.Business;
using Business.Interfaces.Implements.Persons;
using Business.Interfaces.Implements.SecurityAuthentication;
using Business.Interfaces.PDF;
using Business.Repository;
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
        private readonly IObligationMonthService _obligationMonthService;
        private readonly IUserContextService _userContextService;
        private readonly IContractPdfGeneratorService _contractPdfService;
        private readonly ILogger<ContractService> _logger;
        private readonly IMapper _mapper;

        public ContractService(
            IContractRepository contractRepository,
            IPersonService personService,
            IEstablishmentService establishmentService,
            IUserService userService,
            IMapper mapper,
            ISendCode emailService,
            ApplicationDbContext context,
            ICurrentUser user,
            IObligationMonthService obligationMonthService,
            IUserContextService userContextService,
            IContractPdfGeneratorService contractPdfService,
            IUnitOfWork uow,
            ILogger<ContractService> logger
        ) : base(contractRepository, mapper)
        {
            _contractRepository = contractRepository;
            _personService = personService;
            _establishmentService = establishmentService;
            _userService = userService;
            _emailService = emailService;
            _context = context;
            _user = user;
            _obligationMonthService = obligationMonthService;
            _userContextService = userContextService;
            _contractPdfService = contractPdfService;
            _uow = uow;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<ContractCardDto>> GetMineAsync()
        {
            var contracts = _user.EsAdministrador
                ? await _contractRepository.GetCardsAllAsync()
                : await _contractRepository.GetCardsByPersonAsync(
                    _user.PersonId ?? throw new BusinessException("Usuario sin persona asociada."));

            return contracts.ToList().AsReadOnly();
        }

        public async Task<int> CreateContractWithPersonHandlingAsync(ContractCreateDto dto)
        {
            ValidatePayload(dto);

            var personPayload = _mapper.Map<PersonDto>(dto);
            var person = await _personService.GetOrCreateByDocumentAsync(personPayload);

            var (baseRent, uvtQty) = await _establishmentService.ReserveForContractAsync(dto.EstablishmentIds);

            var userResult = await _userService.EnsureUserForPersonAsync(person.Id, dto.Email);

            var contract = BuildContract(dto, person.Id, baseRent, uvtQty);
            await _contractRepository.AddAsync(contract);

            var snapshot = await BuildSnapshotAsync(contract);
            var fullName = ComposeFullName(person);

            SchedulePostCommit(contract.Id, userResult, dto.Email, fullName, snapshot);

            return contract.Id;
        }

        public async Task<IReadOnlyList<ObligationMonthSelectDto>> GetObligationsAsync(int contractId)
        {
            if (contractId <= 0)
                throw new BusinessException("ContractId inválido.");

            return await _obligationMonthService.GetByContractAsync(contractId);
        }

        public async Task<ExpirationSweepResult> RunExpirationSweepAsync(CancellationToken ct = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _context.Database.BeginTransactionAsync(ct);
                try
                {
                    var deactivated = await _contractRepository.DeactivateExpiredAsync(DateTime.UtcNow);
                    var released = await _contractRepository.ReleaseEstablishmentsForExpiredAsync(DateTime.UtcNow);

                    await tx.CommitAsync(ct);

                    _logger.LogInformation("Barrido de expiración: {Count} contratos desactivados, {Estabs} establecimientos liberados.",
                        deactivated.Count, released);

                    return new ExpirationSweepResult(deactivated, released);
                }
                catch
                {
                    await tx.RollbackAsync(ct);
                    throw;
                }
            });
        }

        // ----------------------- Internals -----------------------

        private void ValidatePayload(ContractCreateDto dto)
        {
            if (dto == null)
                throw new BusinessException("Payload inválido.");

            if (dto.EstablishmentIds is null || dto.EstablishmentIds.Count == 0)
                throw new BusinessException("Debe seleccionar al menos un establecimiento.");
        }

        private Contract BuildContract(
            ContractCreateDto dto,
            int personId,
            decimal totalBaseRent,
            decimal totalUvt)
        {
            var contract = _mapper.Map<Contract>(dto);
            contract.PersonId = personId;
            contract.TotalBaseRentAgreed = totalBaseRent;
            contract.TotalUvtQtyAgreed = totalUvt;

            contract.PremisesLeased = dto.EstablishmentIds
                .Select(eid => new PremisesLeased { EstablishmentId = eid }).ToList();

            if (dto.ClauseIds is { Count: > 0 })
            {
                contract.ContractClauses = dto.ClauseIds
                    .Distinct()
                    .Select(cid => new ContractClause { ClauseId = cid })
                    .ToList();
            }

            return contract;
        }

        private async Task<ContractSelectDto?> BuildSnapshotAsync(Contract contract)
        {
            var loaded = await _contractRepository.GetByIdAsync(contract.Id);
            return _mapper.Map<ContractSelectDto>(loaded ?? contract);
        }

        private string ComposeFullName(PersonSelectDto person)
        {
            return $"{person.FirstName} {person.LastName}".Trim();
        }

        private void SchedulePostCommit(
            int contractId,
            (int userId, bool created, string? tempPassword) userResult,
            string? email,
            string fullName,
            ContractSelectDto? contractSnapshot)
        {
            if (userResult.created)
            {
                _uow.RegisterPostCommit(ct =>
                {
                    try
                    {
                        _userContextService.InvalidateCache(userResult.userId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error invalidando cache para usuario {UserId}", userResult.userId);
                    }

                    return Task.CompletedTask;
                });

                if (!string.IsNullOrWhiteSpace(userResult.tempPassword) && !string.IsNullOrWhiteSpace(email))
                {
                    SendTempPasswordPostCommit(email!, fullName, userResult.tempPassword!);
                }
            }

            GenerateObligationPostCommit(contractId);
            SendContractEmailPostCommit(contractId, email, fullName, contractSnapshot);
        }

        private void SendTempPasswordPostCommit(string email, string fullName, string tempPassword)
        {
            RunPostCommitBackground(() =>
                _emailService.SendTemporaryPasswordAsync(email, fullName, tempPassword),
                "Error enviando contraseña temporal a {Email} para {Name}",
                email, fullName);
        }

        private void GenerateObligationPostCommit(int contractId)
        {
            _uow.RegisterPostCommit(async ct =>
            {
                try
                {
                    var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                        TimeZoneConverter.TZConvert.GetTimeZoneInfo("America/Bogota"));

                    await _obligationMonthService.GenerateForContractMonthAsync(contractId, now.Year, now.Month);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generando obligaciones para contrato {ContractId}", contractId);
                }
            });
        }

        private void SendContractEmailPostCommit(
            int contractId,
            string? email,
            string fullName,
            ContractSelectDto? contractSnapshot)
        {
            if (string.IsNullOrWhiteSpace(email) || contractSnapshot == null)
                return;

            var contractNumber = contractId.ToString("D6");

            RunPostCommitBackground(async () =>
            {
                var pdf = await _contractPdfService.GeneratePdfAsync(contractSnapshot);
                await _emailService.SendContractWithPdfAsync(email!, fullName, contractNumber, pdf);
            }, "Error enviando PDF del contrato {ContractId} a {Email}", contractId, email);
        }

        private void RunPostCommitBackground(Func<Task> task, string errorMsg, params object[] args)
        {
            _uow.RegisterPostCommit(ct =>
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await task();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, errorMsg, args.Append(ex).ToArray());
                    }
                }, ct);

                return Task.CompletedTask;
            });
        }
    }
}
